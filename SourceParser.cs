using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static SaintsFieldSourceParser.Utils;

namespace SaintsFieldSourceParser
{
    [Generator]
    public class SourceParser : ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            string pathToSave = null;
            // bool found = false;
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                if (file.Path.EndsWith("Config.SaintsFieldSourceParser.additionalfile"))
                {
                    // Utils.DebugToFile($"==={file.GetText()}");
                    // found = true;
                    foreach (TextLine textLine in file.GetText().Lines)
                    {
                        // DebugToFile(textLine.ToString());
                        string[] split = textLine.ToString().Split('=');
                        if (split.Length == 2)
                        {
                            string key = split[0].Trim();
                            string value = split[1].Trim();
                            if (key == "path")
                            {
                                pathToSave = value;
                            }
                        }
                    }
                    break;
                }

                // Utils.DebugToFile($"---file={file.Path}");
                // Utils.DebugToFile($"==={file.GetText()}");

                // if (Path.GetFileName(file.Path) == "myconfig.json")
                // {
                //     var text = file.GetText(context.CancellationToken)?.ToString();
                // }
            }

            if (pathToSave is null)
            {
                DebugToFile($"!!!!!!!!!!!!!NOTFOUND");
                return;
            }

            try
            {
                // string nameSpace = "";

                foreach (SyntaxTree tree in context.Compilation.SyntaxTrees)
                {
                    // DebugToFile($"Processing {tree.FilePath}");
                    // if (tree.FilePath.Contains("Part1"))
                    // {
                    //     DebugToFile(tree.FilePath);
                    // }
                    SourceText csText = tree.GetText();
                    ImmutableArray<byte> csCheckSum = csText.GetChecksum();
                    string csB64 = Convert.ToBase64String(csCheckSum.ToArray());

                    // string fileBaseName = Path.GetFileNameWithoutExtension(tree.FilePath);

                    CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                    SemanticModel semanticModel = context.Compilation.GetSemanticModel(tree);

                    foreach (MemberDeclarationSyntax memberDeclarationSyntax in root.Members)
                    {
                        // if (tree.FilePath.Contains("Part1"))
                        // {
                        //     DebugToFile($"memberDeclarationSyntax.Kind()={memberDeclarationSyntax.Kind()}");
                        // }
                        // DebugToFile($"memberDeclarationSyntax.Kind()={memberDeclarationSyntax.Kind()}");
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (memberDeclarationSyntax.Kind())
                        {
                            case SyntaxKind.NamespaceDeclaration:
                                {
                                    NamespaceDeclarationSyntax namespaceDeclarationSyntax =
                                        (NamespaceDeclarationSyntax)memberDeclarationSyntax;

                                    ParseNamespace(pathToSave, csB64, context.Compilation, semanticModel, namespaceDeclarationSyntax);

                                    // DebugToFile($"Processing namespace {namespaceDeclarationSyntax.Name}");
                                    // nameSpace = namespaceDeclarationSyntax.Name.ToString();
                                    // if(namespaceDeclarationSyntax.)

                                    // ScoopedWriter nameSpaceResult =
                                    //     ParseNamespace(context.Compilation, semanticModel, namespaceDeclarationSyntax);
                                    // if (nameSpaceResult != null)
                                    // {
                                    //     writers.Add(nameSpaceResult);
                                    // }

                                }
                                break;
                            case SyntaxKind.ClassDeclaration:
                            {
                                // DebugToFile($"try save to folder {pathToSave}: {Directory.Exists(pathToSave)}");
                                // DebugToFile($"class: {memberDeclarationSyntax.Na}");
                                ClassDeclarationSyntax classDecl =
                                    (ClassDeclarationSyntax)memberDeclarationSyntax;

                                INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classDecl);

                                ParseClassOrStructDeclarationSyntax(pathToSave, csB64, context.Compilation, classSymbol);

                                // INamedTypeSymbol classSymbol = classS.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

                                // INamespaceSymbol nameS = classSymbol.ContainingNamespace;
                                // string nameSpacePrefix = string.IsNullOrEmpty(nameSpace)
                                //     ? ""
                                //     : $"{nameSpace}.";
                                //
                                //
                                // DebugToFile(saveFile);
                                // string fullName = classSymbol.ToDisplayString();

                                // var format = new SymbolDisplayFormat(
                                //     typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                                //     genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                                // );
                                //
                                //
                                //
                                // string assemblyName = classSymbol.ContainingAssembly.Name;
                                //
                                //
                                //
                                // var assFolder = $"{pathToSave}/{assemblyName}";
                                //
                                // if (tree.FilePath.Contains("Part1"))
                                // {
                                //     DebugToFile(assFolder);
                                // }
                                //
                                // if (!Directory.Exists(assFolder))
                                // {
                                //     try
                                //     {
                                //         Directory.CreateDirectory(assFolder);
                                //     }
                                //     catch (Exception e)
                                //     {
                                //         DebugToFile(e.Message);
                                //         continue;
                                //     }
                                // }
                                //
                                //
                                // string fullGenericName = classSymbol.ToDisplayString(format);
                                //
                                // var saveFile = $"{assFolder}/{fullGenericName.Replace("<", "_").Replace(">", "_")}.rc";
                                //
                                // // DebugToFile(classDecl.ToFullString());
                                // try
                                // {
                                //     File.WriteAllText(saveFile, $"{csB64}\n{tree.FilePath}");
                                // }
                                // catch (Exception e)
                                // {
                                //     DebugToFile(e.Message);
                                // }
                            }
                                break;
                            case SyntaxKind.StructDeclaration:
                            {
                                StructDeclarationSyntax structDecl = (StructDeclarationSyntax)memberDeclarationSyntax;
                                INamedTypeSymbol structSymbol = semanticModel.GetDeclaredSymbol(structDecl);

                                ParseClassOrStructDeclarationSyntax(pathToSave, csB64, context.Compilation, structSymbol);
                                // ClassOrStructWriter structResult =
                                //     ParseClassOrStructDeclarationSyntax(context.Compilation, ModelExtensions.GetDeclaredSymbol(semanticModel, structDecl));
                                // if (structResult != null)
                                // {
                                //     writers.Add(structResult);
                                // }
                            }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugToFile(e.Message);
                DebugToFile(e.StackTrace);
            }

            // foreach (SyntaxTree compilationSyntaxTree in context.Compilation.SyntaxTrees)
            // {
            //     Utils.DebugToFile($"---{compilationSyntaxTree.FilePath}");
            // }

            // foreach (var kv in context.AnalyzerConfigOptions.GlobalOptions.Keys)
            // {
            //     context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(kv, out var value);
            //     Utils.DebugToFile($"{kv}={value}");
            //
            // }
            // Utils.DebugToFile($"end");
            // context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            //     "build_property.UnityProjectGuid",
            //
            //     out var projectPath
            // );
            // Utils.DebugToFile($"target folder: {projectPath}");
            // try
            // {
            //
            //     string root = GetUnityProjectRoot(context.Compilation);
            //     // int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            //     string tempFolderPath = Path.GetTempPath();
            //     // string targetFolder = Path.Combine(tempFolderPath, $"SaintsFieldParser-{pid}");
            //     Utils.DebugToFile($"target folder: {root}");
            // }
            // catch (Exception e)
            // {
            //     Utils.DebugToFile(e.Message);
            // }
            // if (!Directory.Exists(targetFolder))
            // {
            //     Directory.CreateDirectory(targetFolder);
            // }
        }

        private void ParseNamespace(string pathToSave, string csB64, Compilation compilation, SemanticModel semanticModel, NamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {
            foreach (MemberDeclarationSyntax memberDeclarationSyntax in namespaceDeclarationSyntax.Members)
            {
                // DebugToFile($"Processing {memberDeclarationSyntax.Kind()} in namespace {namespaceDeclarationSyntax.Name}");
                switch (memberDeclarationSyntax.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                    {
                        ClassDeclarationSyntax classDeclaration =
                            (ClassDeclarationSyntax)memberDeclarationSyntax;
                        // DebugToFile($"Processing class {classDeclaration.Identifier} in namespace {namespaceDeclarationSyntax.Name}");
                        ParseClassOrStructDeclarationSyntax(pathToSave, csB64, compilation, semanticModel.GetDeclaredSymbol(classDeclaration));
                    }
                        break;
                    case SyntaxKind.StructDeclaration:
                    {
                        StructDeclarationSyntax structDeclaration =
                            (StructDeclarationSyntax)memberDeclarationSyntax;
                        ParseClassOrStructDeclarationSyntax(pathToSave, csB64, compilation, semanticModel.GetDeclaredSymbol(structDeclaration));
                    }
                        break;
                }
            }

        }

        private void ParseClassOrStructDeclarationSyntax(string pathToSave, string csB64, Compilation compilation, INamedTypeSymbol namedTypeSymbol)
        {
            SymbolDisplayFormat format = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
            );
            string assemblyName = namedTypeSymbol.ContainingAssembly.Name;
            if (assemblyName.StartsWith("Unity.") || assemblyName.StartsWith("UnityEngine.") ||
                assemblyName.StartsWith("UnityEditor."))
            {
                return;
            }

            string assFolder = $"{pathToSave}/{assemblyName}";

            if (!Directory.Exists(assFolder))
            {
                try
                {
                    Directory.CreateDirectory(assFolder);
                }
                catch (Exception e)
                {
                    DebugToFile(e.Message);
                    return;
                }
            }

            string fullGenericName = namedTypeSymbol.ToDisplayString(format);
            // DebugToFile(fullGenericName);
            // if (fullGenericName.Contains('<'))
            // {
            //     DebugToFile(fullGenericName);
            // }

            string saveFile = $"{assFolder}/{GetWriteFileName(fullGenericName)}.rc";

            List<string> lines = new List<string>
            {
                "0",
                csB64,
            };
            // DebugToFile(classDecl.ToFullString());
            // DebugToFile($"write to file {saveFile}");
            foreach (ISymbol member in namedTypeSymbol.GetMembers())
            {
                switch (member)
                {
                    case IFieldSymbol fieldSymbol:
                    {
                        // DebugToFile($"Get IFieldSymbol {fieldSymbol.Name} in {namedTypeSymbol.Name}");
                        string fieldType = fieldSymbol.Type.ToDisplayString(format);
                        string simpleName = (fieldSymbol.AssociatedSymbol is IPropertySymbol prop)
                            ? prop.Name
                            : fieldSymbol.Name;
                        lines.Add($"Field {fieldType} | {simpleName}");
                    }
                        break;
                    case IPropertySymbol propertySymbol:
                    {
                        // DebugToFile($"Get IPropertySymbol {propertySymbol.Name}");
                        string propertyType = propertySymbol.Type.ToDisplayString(format);
                        string propertyName = propertySymbol.Name;
                        lines.Add($"Property {propertyType} | {propertyName}");
                    }
                        break;
                    case IMethodSymbol methodSymbol:
                    {
                        // DebugToFile($"Get IMethodSymbol {methodSymbol.Name}");
                        string methodName = methodSymbol.Name;
                        if (methodName.Contains('.'))
                        {
                            break;
                        }
                        string methodReturnType = methodSymbol.ReturnType.ToDisplayString(format);

                        List<string> methodParameterTypes = new List<string>();
                        foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                        {
                            string paraType = parameterSymbol.Type.ToDisplayString(format);
                            methodParameterTypes.Add(paraType);
                        }
                        lines.Add($"Method {methodReturnType} | {methodName} | {string.Join(" ; ", methodParameterTypes)}");
                    }
                        break;
                }
            }
            try
            {
                File.WriteAllLines(saveFile, lines);
            }
            catch (Exception e)
            {
                DebugToFile(e.Message);
            }

            foreach (INamedTypeSymbol subTypeSymbol in namedTypeSymbol.GetTypeMembers())
            {
                // DebugToFile($"namedTypeSymbol.Name={namedTypeSymbol.Name}");
                ParseClassOrStructDeclarationSyntax(pathToSave, csB64, compilation, subTypeSymbol);
            }
        }

        private static string GetWriteFileName(string fullGenericName)
        {
            return Regex.Replace(
                fullGenericName,
                "<([^<>]+)>",
                m =>
                {
                    // Count generic parameters by commas
                    int genericCount = m.Groups[1].Value.Split(',').Length;
                    return new string(',', genericCount);
                });
            // int leftIndex = fullGenericName.IndexOf('<');
            // if (leftIndex < 0)
            // {
            //     return fullGenericName;
            // }
            //
            // // DebugToFile(fullGenericName);
            //
            // string baseName = fullGenericName.Substring(0, leftIndex);
            // string genericName = fullGenericName.Substring(leftIndex + 1, fullGenericName.Length - leftIndex - 1);
            // int genericCount = genericName.Count(c => c == ',');
            // return baseName + new string(',', genericCount + 1);
        }
    }
}
