#load "XmlEditor.fs"
#r "System.Xml.Linq.dll"

open XmlEditor
open System.Xml.Linq

let nugetXmlns = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"
let editProject rawProjectXml =
   let project = XDocument.Parse rawProjectXml
   query {
      for file in project.Descendants (XName.Get ("file", nugetXmlns)) do
      for attribute in file.Attributes () do
      select attribute
   }
   |> Seq.iter (fun attribute -> attribute.Value <- attribute.Value.Replace ('\\', '/'))
   project

main editProject
