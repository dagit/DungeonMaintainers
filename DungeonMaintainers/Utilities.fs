module Utilities

open System
open FSharp.Data

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

/// Prints a formatted string to DebugListeners.
let inline dprintfn fmt =
    Printf.ksprintf System.Diagnostics.Debug.WriteLine fmt

type TextureAtlas = XmlProvider<"""
<TextureAtlas imagePath="foo.png" width="100" height="100">
  <SubTexture name="region1" x="0"  y="0"  width="10" height="10" />
  <SubTexture name="region2" x="10" y="10" width="20" height="20" offset-x="2" />
  <SubTexture name="region3" x="10" y="10" width="20" height="20" offset-y="3" />
  <SubTexture name="region4" x="10" y="10" width="20" height="20" offset-x="2" offset-y="3" />
</TextureAtlas>
""">

/// Utility function for consuming an option.
let option (def: 'b) (f: 'a -> 'b) (opt: 'a option) =
    match opt with
    | Some a -> f a
    | None   -> def

type Tile(tx, ty, w, h, ox, oy, tex: Texture2D) =
    let sourceRect = Nullable<Microsoft.Xna.Framework.Rectangle>(Microsoft.Xna.Framework.Rectangle(tx, ty, w, h))

    new (tx, ty, w, h, tex: Texture2D) =
        Tile(tx, ty, w, h, 0.0f, 0.0f, tex)

    new (tex: Texture2D, sub: TextureAtlas.SubTexture) =
        Tile(int sub.X, int sub.Y, int sub.Width, int sub.Height,
                option 0.0f float32 sub.OffsetX,
                option 0.0f float32 sub.OffsetY, tex)

    member this.Texture = tex
    member this.Source  = sourceRect
    member __.Width  with get () = w
    member __.Height with get () = h


type TextureAtlasTiles(content : ContentManager, path : string) =
    let atlas = TextureAtlas.Load(new IO.StreamReader(path))
    let img   = content.Load<Texture2D>(atlas.ImagePath)
    let fw    = float32 atlas.Width
    let fh    = float32 atlas.Height
    let tiles = seq { for child in atlas.SubTextures do
                          let tile = new Tile(img,child)
                          yield child.Name, tile } |> Map.ofSeq

    member __.Width  with get () = fw
    member __.Height with get () = fh

    member __.GetTile str = Map.find str tiles
    member __.TryGetTile str = Map.tryFind str tiles

/// Simple spritesheets, where the spritesheet is packed with tiles of a
/// constant size, with a fixed margin between them.
type SimpleTiles(content: ContentManager, path: string, w, h, border) =
    let img = content.Load<Texture2D>(path)

    let xinc = w + border
    let yinc = h + border

    let cols = img.Width  / xinc + 1
    let rows = img.Height / yinc + 1

    let tiles = seq { for row in 0 .. rows - 1 do
                          for col in 0 .. cols - 1 do
                              let x = col * xinc
                              let y = row * yinc
                              let name = sprintf "%d,%d" row col
                              let tile = new Tile (x,y,w,h,img)
                              yield name, tile } |> Map.ofSeq

    member __.GetTile    str = Map.find    str tiles
    member __.TryGetTile str = Map.tryFind str tiles

    member __.Width  with get () = w
    member __.Height with get () = h