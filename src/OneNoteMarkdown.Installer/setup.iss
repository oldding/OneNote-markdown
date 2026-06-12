[Setup]
AppId={{7D0C9CB6-5D83-4E17-89D5-1C8A285F52B4}
AppName=OneNote Markdown
AppVersion=1.0.0
AppPublisher=OneNote Markdown
DefaultDirName={autopf}\OneNoteMarkdown
DefaultGroupName=OneNote Markdown
DisableProgramGroupPage=yes
OutputBaseFilename=OneNoteMarkdownSetup
ArchitecturesAllowed=x86 x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Files]
Source: "..\OneNoteMarkdown.AddIn\bin\Release\OneNoteMarkdown.AddIn.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\WpfMath.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\XamlMath.Shared.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\Extensibility.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\office.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\Microsoft.Office.Interop.OneNote.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\OneNoteMarkdown.AddIn\bin\Release\stdole.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\OneNoteMarkdown.AddIn\bin\Release\*.pdb"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\..\HELP.md"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Office\OneNote\AddIns\OneNoteMarkdown.Connect"; ValueType: string; ValueName: "Description"; ValueData: "OneNote Markdown - Markdown 工具插件"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Office\OneNote\AddIns\OneNoteMarkdown.Connect"; ValueType: string; ValueName: "FriendlyName"; ValueData: "OneNote Markdown"
Root: HKCU; Subkey: "Software\Microsoft\Office\OneNote\AddIns\OneNoteMarkdown.Connect"; ValueType: dword; ValueName: "LoadBehavior"; ValueData: "3"

Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"; ValueType: string; ValueData: "OneNoteMarkdown.AddIn.Connect"; Flags: uninsdeletekey
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"; ValueType: string; ValueName: "AppID"; ValueData: "{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\Implemented Categories\{{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}"; Flags: uninsdeletekey
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueData: "mscoree.dll"; Flags: uninsdeletekey
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueName: "ThreadingModel"; ValueData: "Both"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueName: "Class"; ValueData: "OneNoteMarkdown.AddIn.Connect"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueName: "Assembly"; ValueData: "OneNoteMarkdown.AddIn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueName: "RuntimeVersion"; ValueData: "v4.0.30319"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32"; ValueType: string; ValueName: "CodeBase"; ValueData: "{app}\OneNoteMarkdown.AddIn.dll"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32\1.0.0.0"; ValueType: string; ValueName: "Class"; ValueData: "OneNoteMarkdown.AddIn.Connect"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32\1.0.0.0"; ValueType: string; ValueName: "Assembly"; ValueData: "OneNoteMarkdown.AddIn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32\1.0.0.0"; ValueType: string; ValueName: "RuntimeVersion"; ValueData: "v4.0.30319"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\InprocServer32\1.0.0.0"; ValueType: string; ValueName: "CodeBase"; ValueData: "{app}\OneNoteMarkdown.AddIn.dll"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\ProgId"; ValueType: string; ValueData: "OneNoteMarkdown.Connect"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\Programmable"; ValueType: string; ValueData: ""
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\TypeLib"; ValueType: string; ValueData: "{{5C3E37E8-7D8A-41E5-9D3D-6B5A1C92B7D1}"
Root: HKCR; Subkey: "CLSID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}\VersionIndependentProgID"; ValueType: string; ValueData: "OneNoteMarkdown.Connect"

Root: HKCR; Subkey: "AppID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"; ValueType: string; ValueData: "OneNoteMarkdown.AddIn"; Flags: uninsdeletekey
Root: HKCR; Subkey: "AppID\{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"; ValueType: string; ValueName: "DllSurrogate"; ValueData: ""
Root: HKCR; Subkey: "AppID\OneNoteMarkdown.AddIn.dll"; ValueType: string; ValueName: "AppID"; ValueData: "{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"; Flags: uninsdeletekey

Root: HKCR; Subkey: "TypeLib\{{5C3E37E8-7D8A-41E5-9D3D-6B5A1C92B7D1}\1.0"; ValueType: string; ValueData: "OneNote Markdown 插件"; Flags: uninsdeletekey
Root: HKCR; Subkey: "TypeLib\{{5C3E37E8-7D8A-41E5-9D3D-6B5A1C92B7D1}\1.0\0\win64"; ValueType: string; ValueData: "{app}\OneNoteMarkdown.AddIn.tlb"
Root: HKCR; Subkey: "TypeLib\{{5C3E37E8-7D8A-41E5-9D3D-6B5A1C92B7D1}\1.0\FLAGS"; ValueType: string; ValueData: "0"
Root: HKCR; Subkey: "TypeLib\{{5C3E37E8-7D8A-41E5-9D3D-6B5A1C92B7D1}\1.0\HELPDIR"; ValueType: string; ValueData: "{app}"

Root: HKCR; Subkey: "OneNoteMarkdown.Connect"; ValueType: string; ValueData: "OneNoteMarkdown.AddIn.Connect"; Flags: uninsdeletekey
Root: HKCR; Subkey: "OneNoteMarkdown.Connect\CLSID"; ValueType: string; ValueData: "{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"
Root: HKCR; Subkey: "OneNoteMarkdown.Connect\CurVer"; ValueType: string; ValueData: "OneNoteMarkdown.Connect.1"
Root: HKCR; Subkey: "OneNoteMarkdown.Connect.1"; ValueType: string; ValueData: "OneNoteMarkdown.AddIn.Connect"; Flags: uninsdeletekey
Root: HKCR; Subkey: "OneNoteMarkdown.Connect.1\CLSID"; ValueType: string; ValueData: "{{0A92B61B-98B8-4E5D-BE2D-48EDB01ED177}"

[UninstallRun]
Filename: "{dotnet4064}\RegAsm.exe"; Parameters: "/u ""{app}\OneNoteMarkdown.AddIn.dll"""; StatusMsg: "正在注销 64 位 COM 组件..."; Flags: runhidden waituntilterminated skipifdoesntexist; Check: IsWin64
Filename: "{dotnet40}\RegAsm.exe"; Parameters: "/u ""{app}\OneNoteMarkdown.AddIn.dll"""; StatusMsg: "正在注销 32 位 COM 组件..."; Flags: runhidden waituntilterminated skipifdoesntexist

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
const
  DotNet48Release = 528040;

function IsDotNet48Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and
    (Release >= DotNet48Release);

  if (not Result) and IsWin64 then
    Result := RegQueryDWordValue(HKLM64, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and
      (Release >= DotNet48Release);
end;

function InitializeSetup: Boolean;
begin
  Result := IsDotNet48Installed;
  if not Result then
  begin
    MsgBox('需要先安装 .NET Framework 4.8 才能继续安装 OneNote Markdown。', mbCriticalError, MB_OK);
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, 'Software\Microsoft\Office\OneNote\AddInsData\OneNoteMarkdown.Connect');
  end;
end;
