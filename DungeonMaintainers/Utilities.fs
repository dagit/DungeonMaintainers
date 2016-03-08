module Utilities

/// Prints a formatted string to DebugListeners.
let inline dprintfn fmt =
    Printf.ksprintf System.Diagnostics.Debug.WriteLine fmt