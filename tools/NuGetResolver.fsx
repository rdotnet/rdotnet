#load "XmlEditor.fs"
#r "System.Xml.Linq.dll"

open XmlEditor
open System.Xml.Linq

let editProject rawProjectXml =
   let project = XDocument.Parse rawProjectXml
   query {
      for file in project.Descendants (XName.Get ("file")) do
      for attribute in file.Attributes () do
      select attribute
   }
   |> Seq.iter (fun attribute -> attribute.Value <- attribute.Value.Replace ('\\', '/'))
   project

main editProject
