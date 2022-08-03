module PlaygroundTests.Tests

open System
open System.Diagnostics.Tracing
open System.IO
open System.Diagnostics
open FSharp.Compiler.Diagnostics
open NUnit.Framework
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Service.Tests.Common

let nugetCache =
    let ps = ProcessStartInfo("dotnet", "nuget locals global-packages -l")
    ps.RedirectStandardOutput <- true
    ps.RedirectStandardError <- true
    let p = Process.Start ps
    let stdout = p.StandardOutput.ReadToEnd().Trim()
    p.WaitForExit()
    stdout.Replace("global-packages:", "").Trim()

let (</>) a b = Path.Combine(a, b)
let projectDirectory = __SOURCE_DIRECTORY__ </> "FSharpPlaygroundProject"
let projectFileName = projectDirectory </> "FSharpPlaygroundProject.fsproj"
let libraryFile = projectDirectory </> "Library.fs"
let targetFramework = "net6.0"
let objDirectory = projectDirectory </> "obj" </> "Debug" </> targetFramework
let outputFile = objDirectory </> "FSharpPlaygroundProject.dll"
let xmlFile = objDirectory </> "FSharpPlaygroundProject.xml"
let fsharpCoreVersion = "6.0.5"

let fsharpReference =
    nugetCache
    </> "fsharp.core"
    </> fsharpCoreVersion
    </> "lib"
    </> "netstandard2.1"
    </> "FSharp.Core.dll"

let dotnetPacksDirectory = @"C:\Program Files\dotnet\packs"
let microsoftNETCoreAppRefVersion = "6.0.7"

// These assemblies are included by MSBuild for a default library project
let sdkAssemblies =
    [
        "Microsoft.CSharp.dll"
        "Microsoft.VisualBasic.Core.dll"
        "Microsoft.VisualBasic.dll"
        "Microsoft.Win32.Primitives.dll"
        "Microsoft.Win32.Registry.dll"
        "mscorlib.dll"
        "netstandard.dll"
        "System.AppContext.dll"
        "System.Buffers.dll"
        "System.Collections.Concurrent.dll"
        "System.Collections.dll"
        "System.Collections.Immutable.dll"
        "System.Collections.NonGeneric.dll"
        "System.Collections.Specialized.dll"
        "System.ComponentModel.Annotations.dll"
        "System.ComponentModel.DataAnnotations.dll"
        "System.ComponentModel.dll"
        "System.ComponentModel.EventBasedAsync.dll"
        "System.ComponentModel.Primitives.dll"
        "System.ComponentModel.TypeConverter.dll"
        "System.Configuration.dll"
        "System.Console.dll"
        "System.Core.dll"
        "System.Data.Common.dll"
        "System.Data.DataSetExtensions.dll"
        "System.Data.dll"
        "System.Diagnostics.Contracts.dll"
        "System.Diagnostics.Debug.dll"
        "System.Diagnostics.DiagnosticSource.dll"
        "System.Diagnostics.FileVersionInfo.dll"
        "System.Diagnostics.Process.dll"
        "System.Diagnostics.StackTrace.dll"
        "System.Diagnostics.TextWriterTraceListener.dll"
        "System.Diagnostics.Tools.dll"
        "System.Diagnostics.TraceSource.dll"
        "System.Diagnostics.Tracing.dll"
        "System.dll"
        "System.Drawing.dll"
        "System.Drawing.Primitives.dll"
        "System.Dynamic.Runtime.dll"
        "System.Formats.Asn1.dll"
        "System.Globalization.Calendars.dll"
        "System.Globalization.dll"
        "System.Globalization.Extensions.dll"
        "System.IO.Compression.Brotli.dll"
        "System.IO.Compression.dll"
        "System.IO.Compression.FileSystem.dll"
        "System.IO.Compression.ZipFile.dll"
        "System.IO.dll"
        "System.IO.FileSystem.AccessControl.dll"
        "System.IO.FileSystem.dll"
        "System.IO.FileSystem.DriveInfo.dll"
        "System.IO.FileSystem.Primitives.dll"
        "System.IO.FileSystem.Watcher.dll"
        "System.IO.IsolatedStorage.dll"
        "System.IO.MemoryMappedFiles.dll"
        "System.IO.Pipes.AccessControl.dll"
        "System.IO.Pipes.dll"
        "System.IO.UnmanagedMemoryStream.dll"
        "System.Linq.dll"
        "System.Linq.Expressions.dll"
        "System.Linq.Parallel.dll"
        "System.Linq.Queryable.dll"
        "System.Memory.dll"
        "System.Net.dll"
        "System.Net.Http.dll"
        "System.Net.Http.Json.dll"
        "System.Net.HttpListener.dll"
        "System.Net.Mail.dll"
        "System.Net.NameResolution.dll"
        "System.Net.NetworkInformation.dll"
        "System.Net.Ping.dll"
        "System.Net.Primitives.dll"
        "System.Net.Requests.dll"
        "System.Net.Security.dll"
        "System.Net.ServicePoint.dll"
        "System.Net.Sockets.dll"
        "System.Net.WebClient.dll"
        "System.Net.WebHeaderCollection.dll"
        "System.Net.WebProxy.dll"
        "System.Net.WebSockets.Client.dll"
        "System.Net.WebSockets.dll"
        "System.Numerics.dll"
        "System.Numerics.Vectors.dll"
        "System.ObjectModel.dll"
        "System.Reflection.DispatchProxy.dll"
        "System.Reflection.dll"
        "System.Reflection.Emit.dll"
        "System.Reflection.Emit.ILGeneration.dll"
        "System.Reflection.Emit.Lightweight.dll"
        "System.Reflection.Extensions.dll"
        "System.Reflection.Metadata.dll"
        "System.Reflection.Primitives.dll"
        "System.Reflection.TypeExtensions.dll"
        "System.Resources.Reader.dll"
        "System.Resources.ResourceManager.dll"
        "System.Resources.Writer.dll"
        "System.Runtime.CompilerServices.Unsafe.dll"
        "System.Runtime.CompilerServices.VisualC.dll"
        "System.Runtime.dll"
        "System.Runtime.Extensions.dll"
        "System.Runtime.Handles.dll"
        "System.Runtime.InteropServices.dll"
        "System.Runtime.InteropServices.RuntimeInformation.dll"
        "System.Runtime.Intrinsics.dll"
        "System.Runtime.Loader.dll"
        "System.Runtime.Numerics.dll"
        "System.Runtime.Serialization.dll"
        "System.Runtime.Serialization.Formatters.dll"
        "System.Runtime.Serialization.Json.dll"
        "System.Runtime.Serialization.Primitives.dll"
        "System.Runtime.Serialization.Xml.dll"
        "System.Security.AccessControl.dll"
        "System.Security.Claims.dll"
        "System.Security.Cryptography.Algorithms.dll"
        "System.Security.Cryptography.Cng.dll"
        "System.Security.Cryptography.Csp.dll"
        "System.Security.Cryptography.Encoding.dll"
        "System.Security.Cryptography.OpenSsl.dll"
        "System.Security.Cryptography.Primitives.dll"
        "System.Security.Cryptography.X509Certificates.dll"
        "System.Security.dll"
        "System.Security.Principal.dll"
        "System.Security.Principal.Windows.dll"
        "System.Security.SecureString.dll"
        "System.ServiceModel.Web.dll"
        "System.ServiceProcess.dll"
        "System.Text.Encoding.CodePages.dll"
        "System.Text.Encoding.dll"
        "System.Text.Encoding.Extensions.dll"
        "System.Text.Encodings.Web.dll"
        "System.Text.Json.dll"
        "System.Text.RegularExpressions.dll"
        "System.Threading.Channels.dll"
        "System.Threading.dll"
        "System.Threading.Overlapped.dll"
        "System.Threading.Tasks.Dataflow.dll"
        "System.Threading.Tasks.dll"
        "System.Threading.Tasks.Extensions.dll"
        "System.Threading.Tasks.Parallel.dll"
        "System.Threading.Thread.dll"
        "System.Threading.ThreadPool.dll"
        "System.Threading.Timer.dll"
        "System.Transactions.dll"
        "System.Transactions.Local.dll"
        "System.ValueTuple.dll"
        "System.Web.dll"
        "System.Web.HttpUtility.dll"
        "System.Windows.dll"
        "System.Xml.dll"
        "System.Xml.Linq.dll"
        "System.Xml.ReaderWriter.dll"
        "System.Xml.Serialization.dll"
        "System.Xml.XDocument.dll"
        "System.Xml.XmlDocument.dll"
        "System.Xml.XmlSerializer.dll"
        "System.Xml.XPath.dll"
        "System.Xml.XPath.XDocument.dll"
        "WindowsBase.dll"
    ]

let projectOptions: FSharpProjectOptions =
    let assemblyReferences =
        sdkAssemblies
        |> List.map (fun r ->
            sprintf
                "-r:%s"
                (dotnetPacksDirectory
                 </> "Microsoft.NETCore.App.Ref"
                 </> microsoftNETCoreAppRefVersion
                 </> "ref"
                 </> targetFramework
                 </> r))

    {
        ProjectFileName = projectFileName
        ProjectId = Some(Guid.NewGuid().ToString())
        SourceFiles = [| libraryFile |]
        OtherOptions =
            [|
                $"-o:{outputFile}"
                "--debug:portable"
                "--noframework"
                "--define:TRACE"
                "--define:DEBUG"
                "--define:NET"
                "--define:NET6_0"
                "--define:NETCOREAPP"
                "--define:NET5_0_OR_GREATER"
                "--define:NET6_0_OR_GREATER"
                "--define:NETCOREAPP1_0_OR_GREATER"
                "--define:NETCOREAPP1_1_OR_GREATER"
                "--define:NETCOREAPP2_0_OR_GREATER"
                "--define:NETCOREAPP2_1_OR_GREATER"
                "--define:NETCOREAPP2_2_OR_GREATER"
                "--define:NETCOREAPP3_0_OR_GREATER"
                "--define:NETCOREAPP3_1_OR_GREATER"
                $"--doc:{xmlFile}"
                "--optimize-"
                "--tailcalls-"
                $"-r:{fsharpReference}"
                yield! assemblyReferences
                "--target:library"
                "--warn:3"
                "--warnaserror:3239"
                "--fullpaths"
                "--flaterrors"
                "--highentropyva+"
                "--targetprofile:netcore"
                "--nocopyfsharpcore"
                "--deterministic+"
                "--simpleresolution"
            |]
        ReferencedProjects = Array.empty
        IsIncompleteTypeCheckEnvironment = false
        UseScriptResolutionRules = false
        LoadTime = DateTime.Now
        UnresolvedReferences = None
        OriginalLoadReferences = []
        Stamp = None
    }

let getSourceTextOf (file: string) : ISourceText =
    File.ReadAllText file |> SourceText.ofString

type Listener() =
    inherit EventListener()

    let messages = ResizeArray<string>()
    let mutable source = null

    override _.OnEventSourceCreated newSource =
        if newSource.Name = "FSharpCompiler" then
            ``base``.EnableEvents(newSource, EventLevel.LogAlways, EventKeywords.All)
            source <- newSource

    override _.OnEventWritten eventArgs =
        let payload = eventArgs.Payload

        if payload.Count = 2 then
            match payload[0], payload[1] with
            | :? string as message, (:? int as logCompilerFunctionId) ->
                if
                    logCompilerFunctionId = int LogCompilerFunctionId.Service_IncrementalBuildersCache_BuildingNewCache
                then
                    messages.Add(message)
            | _ -> ()

    interface IDisposable with
        member _.Dispose() =
            if isNull source then () else ``base``.DisableEvents(source)

    member this.Messages = messages

let mutable listener = Unchecked.defaultof<Listener>
let mutable checker = Unchecked.defaultof<FSharpChecker>

[<SetUp>]
let init () =
    listener <- new Listener()
    checker <- FSharpChecker.Create(keepAssemblyContents = true)

[<TearDown>]
let exit () = listener.Dispose()

[<Test>]
let ``Debug ParseAndCheckFileInProject`` () =
    let fileResult, checkResult =
        checker.ParseAndCheckFileInProject(libraryFile, 0, getSourceTextOf libraryFile, projectOptions, "playground")
        |> Async.RunSynchronously

    if fileResult.Diagnostics.Length > 0 then
        Assert.Fail($"Got parsing errors, {fileResult.Diagnostics}")

    match checkResult with
    | FSharpCheckFileAnswer.Succeeded checkFileResults ->
        if checkFileResults.Diagnostics.Length > 0 then
            Assert.Fail($"Got checking errors, {checkFileResults.Diagnostics}")
        else
            ()
    | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail "Unexpected abortion,"

[<Test>]
let ``Print signature of file`` () =
    let fileResult, checkResult =
        checker.ParseAndCheckFileInProject(libraryFile, 0, getSourceTextOf libraryFile, projectOptions, "playground")
        |> Async.RunSynchronously

    if fileResult.Diagnostics.Length > 0 then
        Assert.Fail($"Got parsing errors, {fileResult.Diagnostics}")

    match checkResult with
    | FSharpCheckFileAnswer.Succeeded checkFileResults ->
        if checkFileResults.Diagnostics.Length > 0 then
            Assert.Fail($"Got checking errors, {checkFileResults.Diagnostics}")
        else
            match checkFileResults.GenerateSignature() with
            | None -> Assert.Fail "No signature file was generated"
            | Some signature -> printfn $"%s{string signature}"
    | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail "Unexpected abortion,"

[<Test>]
let ``Debug ParseAndCheckProject`` () =
    let result = checker.ParseAndCheckProject(projectOptions) |> Async.RunSynchronously

    if result.Diagnostics.Length > 0 then
        Assert.Fail($"Got parsing errors, {result.Diagnostics}")
    else
        ()

[<Test>]
let ``Print AST of implementation file`` () =
    let ast =
        getParseResults
            """
let a : int * string * bool = 1, "", false
type T = int * string
"""

    printfn $"%A{ast}"

[<Test>]
let ``Print AST of signature file`` () =
    let ast =
        getParseResultsOfSignatureFile
            """
val a: int * string * bool
type T = int * string
"""

    printfn $"%A{ast}"

[<Test>]
let ``Debug Compile`` () =
    let arguments =
        [| yield! projectOptions.OtherOptions; yield! projectOptions.SourceFiles |]

    let diagnostics, _exitCode = checker.Compile(arguments) |> Async.RunSynchronously

    if diagnostics.Length > 0 then
        Assert.Fail($"Got errors, {diagnostics}")
    else
        ()

[<Test>]
let ``Debug BackgroundCheck`` () =
    let parseResult, checkResult =
        checker.GetBackgroundCheckResultsForFileInProject(libraryFile, projectOptions, "playground")
        |> Async.RunSynchronously

    if parseResult.Diagnostics.Length > 0 then
        Assert.Fail($"Got parse errors, {parseResult.Diagnostics}")

    if checkResult.Diagnostics.Length > 0 then
        Assert.Fail($"Got checking errors, {checkResult.Diagnostics}")
