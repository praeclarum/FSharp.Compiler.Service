namespace Microsoft.FSharp.Compiler
    
type ILAssemblyTypeForwarderComparer () =
    interface System.Collections.Generic.IComparer<string[]*string> with
        member this.Compare (x, y) =
            if x = y then 0
            else if fst x < fst y then -1
            else if fst x = fst y && snd x < snd y then -1
            else 1
