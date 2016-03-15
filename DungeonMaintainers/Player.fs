module Player

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open Utilities

type Player (x, y, sprite : Tile) =
  member val X      = x      with get,set
  member val Y      = y      with get,set
  member val Sprite = sprite with get

  member this.Draw ( spriteBatch : SpriteBatch ) =
    spriteBatch.Draw( this.Sprite.Texture
                    , System.Nullable(new Vector2( float32 <| this.X * this.Sprite.Width
                                                 , float32 <| this.Y * this.Sprite.Width))
                    , System.Nullable(), System.Nullable(), System.Nullable(), 0.0f
                    , System.Nullable(Vector2.One)
                    , System.Nullable(Color.White), SpriteEffects.None, 0.9f )
    
(*
  member this.HandleInput (inputState : InputState) (map : IMap) : bool =
    let player = System.Nullable ( PlayerIndex.One )
    let mutable x = this.X
    let mutable y = this.Y
    let mutable input = false
    if inputState.IsLeft  ( player ) then input <- true; x <- x - 1
    if inputState.IsRight ( player ) then input <- true; x <- x + 1
    if inputState.IsUp    ( player ) then input <- true; y <- y - 1
    if inputState.IsDown  ( player ) then input <- true; y <- y + 1
    if input && map.IsWalkable (x, y)
    then
      this.X <- x
      this.Y <- y
      true
    else false
*)