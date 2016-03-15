module Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open RogueSharp
open RogueSharp.MapCreation
open RogueSharp.Random

open Utilities
open Player

type DungeonMaintainersGame () as this =
  inherit Game()

  let tileW              = 16.0f
  let tileH              = 16.0f

  let mapWidth           = 50
  let mapHeight          = 30

  let mutable worldTiles = Unchecked.defaultof<SimpleTiles>
  let mutable charTiles  = Unchecked.defaultof<SimpleTiles>
  let mutable floor      = Unchecked.defaultof<Tile>
  let mutable wall       = Unchecked.defaultof<Tile>
  let mutable player     = Unchecked.defaultof<Player>

  let mutable map        = Unchecked.defaultof<IMap>

  do this.Content.RootDirectory <- "."
  let graphics                  = new GraphicsDeviceManager(this)
  let mutable spriteBatch       = Unchecked.defaultof<SpriteBatch>

  static member val Random = new DotNetRandom() with get

  override this.Initialize() =
    spriteBatch <- new SpriteBatch(this.GraphicsDevice)
    let mapCreationStrategy =
              new RandomRoomsMapCreationStrategy<Map>( mapWidth, mapHeight, 100, 7, 3 )
    map <- Map.Create( mapCreationStrategy )
    base.Initialize()

  override this.LoadContent() =
    worldTiles     <- new SimpleTiles(this.Content, "roguelikeDungeon_transparent.png", int tileW, int tileH, 1)
    charTiles      <- new SimpleTiles(this.Content, "roguelikeChar_transparent.png",    int tileW, int tileH, 1)
    floor          <- worldTiles.GetTile "2,9"
    wall           <- worldTiles.GetTile "11,22"
    let playerCell = this.GetRandomEmptyCell()
    player         <- new Player(playerCell.X,playerCell.Y, charTiles.GetTile "11,0")
    base.LoadContent()

  override this.Update(gameTime) = 
    base.Update(gameTime)

  override this.Draw (gameTime) =
    this.GraphicsDevice.Clear Color.Black
    spriteBatch.Begin()
    for cell in map.GetAllCells() do
      let tile = if cell.IsWalkable
                   then floor
                   else wall
      let position = Vector2( float32 cell.X * tileW
                            , float32 cell.Y * tileH )
      spriteBatch.Draw( tile.Texture, System.Nullable(position)
                      , System.Nullable(), tile.Source, System.Nullable()
                      , 0.0f, System.Nullable(Vector2.One)
                      , System.Nullable(Color.White), SpriteEffects.None, 1.0f
                      )
      spriteBatch.Draw( player.Sprite.Texture, System.Nullable(new Vector2(float32 player.X * tileW, float32 player.Y * tileH))
                      , System.Nullable(), player.Sprite.Source, System.Nullable()
                      , 0.0f, System.Nullable(Vector2.One)
                      , System.Nullable(Color.White), SpriteEffects.None, 0.9f
                      )
    spriteBatch.End()
    base.Draw(gameTime)

    (* Private methods below here *) 
    member private this.GetRandomEmptyCell () : Cell =
      let random = DungeonMaintainersGame.Random
      let rec findOpenSpot () =
        let x = random.Next 49
        let y = random.Next 29
        if map.IsWalkable(x, y)
        then
          map.GetCell(x, y)
        else
          findOpenSpot ()
      findOpenSpot ()

    member private this.UpdatePlayerFieldOfView =
      map.ComputeFov( player.X, player.Y, 30, true )
      let cells = Seq.filter (fun (c:Cell) -> c.IsInFov) (map.GetAllCells())
      for cell in cells do
        map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true )