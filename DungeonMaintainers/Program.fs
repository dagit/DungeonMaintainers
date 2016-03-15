// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Utilities
open Game

[<EntryPoint>]
let main argv = 
    dprintfn "%A" argv
    use game = new DungeonMaintainersGame()
    game.Run()
    0 // return an integer exit code
