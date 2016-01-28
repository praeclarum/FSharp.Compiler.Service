namespace Microsoft.FSharp.Compiler

type ILAssemblyTypeForwarderComparer =
    class
        new : unit -> ILAssemblyTypeForwarderComparer
    end
    interface System.Collections.Generic.IComparer<string[]*string>
