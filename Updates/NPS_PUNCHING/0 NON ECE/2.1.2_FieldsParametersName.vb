Public Function FieldsParametersName(txtFile As String) As String
Dim FileObj As filesystemobject
Dim FS As Object
Set FileObj = CreateObject("Scripting.Filesystemobject")
Set fso = New filesystemobject
Set FS = FileObj.OpenTextFile(App.Path & "\life\insufld\PolicyInfo\" & txtFile)
FieldsParametersName = FS.ReadLine
Set FileObj = Nothing
End Function