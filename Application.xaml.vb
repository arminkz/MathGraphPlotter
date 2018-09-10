Imports System.Reflection
Imports System.IO

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, New ResolveEventHandler(AddressOf ResolveAssembly)

    End Sub

    'No Need To Copy DLLs To Startup Path (New In Version 10.6)
    Private Shared Function ResolveAssembly(sender As Object, Args As ResolveEventArgs) As Assembly
        Dim ParentAssembly As Assembly = Assembly.GetExecutingAssembly()

        Dim Name = Args.Name.Substring(0, Args.Name.IndexOf(","c)) & ".dll"
        Dim ResourceName = ParentAssembly.GetManifestResourceNames().First(Function(s) s.EndsWith(Name))

        Using Stream As Stream = ParentAssembly.GetManifestResourceStream(ResourceName)
            Dim Block As Byte() = New Byte(Stream.Length - 1) {}
            Stream.Read(Block, 0, Block.Length)
            Return Assembly.Load(Block)
        End Using
    End Function

End Class
