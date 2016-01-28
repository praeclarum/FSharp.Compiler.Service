namespace FSharp.Compiler.Service.iOS.Test

open System

open Foundation
open UIKit

open Microsoft.FSharp.Compiler.SourceCodeServices

[<Register ("ViewController")>]
type ViewController (handle:IntPtr) =
    inherit UIViewController (handle)

    override x.DidReceiveMemoryWarning () =
        // Releases the view if it doesn't have a superview.
        base.DidReceiveMemoryWarning ()
        // Release any cached data, images, etc that aren't in use.

    override x.ViewDidLoad () =
        base.ViewDidLoad ()
        System.Threading.ThreadPool.QueueUserWorkItem (fun _ ->
            try
                let checker = FSharpChecker.Create(keepAssemblyContents=true)

                let parseAndCheckSingleFile (input) = 
                    let file = IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), "fsx")  
                    IO.File.WriteAllText(file, input)
                    // Get context representing a stand-alone (script) file
                    let projOptions = 
                        checker.GetProjectOptionsFromScript(file, input)
                        |> Async.RunSynchronously

                    checker.ParseAndCheckProject(projOptions) 
                    |> Async.RunSynchronously

                let sw = new Diagnostics.Stopwatch ()
                sw.Start ()
                let docs = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments)
                let codePath = IO.Path.Combine (docs, "foo.fsx")
                let outPath = IO.Path.ChangeExtension(codePath, ".exe")
                IO.File.WriteAllText (codePath, "module Foo\nlet x = 42\nprintfn \"Hello, world! %A\" x")

                let parsRes = parseAndCheckSingleFile (IO.File.ReadAllText (codePath))

                for e in parsRes.Errors do
                    printfn "FS ERROR: %O" e.Message

                //let compiler = new Microsoft.FSharp.Compiler.SourceCodeServices.
                //let exitCode, output = compiler.Compile [| "fsc"; "--out:" + outPath; codePath |]
                sw.Stop ()

                printfn "OUTPUT %A in %O " (parsRes.AssemblyContents.ImplementationFiles.Head.Declarations) sw.Elapsed

                //for e in output.Errors do
                //    match e with
                //    | Microsoft.FSharp.Compiler.CompileOps.ErrorOrWarning.Long (_, info) -> printfn "OUTPUT ERROR %A" info.Message
                //    | _ -> printfn "OUTPUT ERROR %O" e
                //for e in output.Warnings do
                //    match e with
                //    | Microsoft.FSharp.Compiler.CompileOps.ErrorOrWarning.Long (_, info) -> printfn "OUTPUT WARNING %A" info.Message
                //    | _ -> printfn "OUTPUT WARNING %O" e
                //let outInfo = new IO.FileInfo(outPath)
                //printfn "OUTPUT %d bytes in %O" (int outInfo.Length) sw.Elapsed
            with ex ->
                Console.WriteLine (ex)
            ) |> ignore

    override x.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation) =
        // Return true for supported orientations
        if UIDevice.CurrentDevice.UserInterfaceIdiom = UIUserInterfaceIdiom.Phone then
           toInterfaceOrientation <> UIInterfaceOrientation.PortraitUpsideDown
        else
           true

