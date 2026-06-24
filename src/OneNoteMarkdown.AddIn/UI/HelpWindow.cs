using System;
using System.Drawing;
using System.Windows.Forms;

namespace OneNoteMarkdown.UI
{
    internal sealed class HelpWindow : Form
    {
        private TreeView _treeView;
        private WebBrowser _browser;
        private Panel _headerPanel;
        private Button _closeButton;

        public HelpWindow()
        {
            InitializeComponents();
            PopulateTree();
            ShowContent("about");
        }

        private void InitializeComponents()
        {
            Text = "OneNote Markdown - 帮助";
            Size = new Size(900, 640);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterScreen;
            Icon = null;
            ShowIcon = false;

            // Header panel
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(104, 33, 122)
            };
            Label headerLabel = new Label
            {
                Text = "✦  使用帮助",
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 12f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            _headerPanel.Controls.Add(headerLabel);
            Controls.Add(_headerPanel);

            // Bottom panel with close button
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(0, 8, 16, 8)
            };
            _closeButton = new Button
            {
                Text = "关闭",
                Size = new Size(80, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                FlatStyle = FlatStyle.System
            };
            _closeButton.Click += (s, e) => Close();
            bottomPanel.Controls.Add(_closeButton);
            _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            bottomPanel.Resize += (s, e) =>
            {
                _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            };
            Controls.Add(bottomPanel);

            // Split container
            SplitContainer splitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 180,
                FixedPanel = FixedPanel.Panel1,
                BorderStyle = BorderStyle.None
            };

            // Left: TreeView
            _treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 9.5f),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(252, 252, 252),
                ShowLines = true,
                ShowRootLines = true,
                HideSelection = false,
                ItemHeight = 24
            };
            _treeView.AfterSelect += TreeView_AfterSelect;
            splitter.Panel1.Controls.Add(_treeView);
            splitter.Panel1.BackColor = Color.FromArgb(252, 252, 252);

            // Right: WebBrowser
            _browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false,
                ScriptErrorsSuppressed = true,
                AllowNavigation = false
            };
            splitter.Panel2.Controls.Add(_browser);

            Controls.Add(splitter);

            // Ensure Z-order: header on top, bottom on bottom
            _headerPanel.BringToFront();
        }

        private void PopulateTree()
        {
            TreeNode root = new TreeNode("目录") { Tag = "about" };

            TreeNode quickStart = new TreeNode("快速入门") { Tag = "quickstart" };
            root.Nodes.Add(quickStart);

            TreeNode features = new TreeNode("功能说明") { Tag = "features" };
            features.Nodes.Add(new TreeNode("渲染整页") { Tag = "feat_renderpage" });
            features.Nodes.Add(new TreeNode("渲染选区") { Tag = "feat_rendersel" });
            features.Nodes.Add(new TreeNode("实时模式") { Tag = "feat_live" });
            features.Nodes.Add(new TreeNode("导入/导出") { Tag = "feat_io" });
            features.Nodes.Add(new TreeNode("LaTeX 公式") { Tag = "feat_latex" });
            features.Nodes.Add(new TreeNode("代码高亮") { Tag = "feat_code" });
            root.Nodes.Add(features);

            TreeNode settings = new TreeNode("设置说明") { Tag = "settings" };
            root.Nodes.Add(settings);

            TreeNode shortcuts = new TreeNode("快捷键") { Tag = "shortcuts" };
            root.Nodes.Add(shortcuts);

            TreeNode faq = new TreeNode("常见问题") { Tag = "faq" };
            root.Nodes.Add(faq);

            TreeNode about = new TreeNode("关于") { Tag = "about" };
            root.Nodes.Add(about);

            _treeView.Nodes.Add(root);
            root.ExpandAll();
            _treeView.SelectedNode = root.Nodes[root.Nodes.Count - 1]; // select "关于"
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string tag = e.Node?.Tag as string;
            if (!string.IsNullOrEmpty(tag))
            {
                ShowContent(tag);
            }
        }

        private void ShowContent(string section)
        {
            string html = BuildHtml(section);
            _browser.DocumentText = html;
        }

        private static string BuildHtml(string section)
        {
            string body;
            switch (section)
            {
                case "about":
                    body = GetAboutHtml();
                    break;
                case "quickstart":
                    body = GetQuickStartHtml();
                    break;
                case "features":
                    body = GetFeaturesOverviewHtml();
                    break;
                case "feat_renderpage":
                    body = GetRenderPageHtml();
                    break;
                case "feat_rendersel":
                    body = GetRenderSelHtml();
                    break;
                case "feat_live":
                    body = GetLiveHtml();
                    break;
                case "feat_io":
                    body = GetIoHtml();
                    break;
                case "feat_latex":
                    body = GetLatexHtml();
                    break;
                case "feat_code":
                    body = GetCodeHtml();
                    break;
                case "settings":
                    body = GetSettingsHtml();
                    break;
                case "shortcuts":
                    body = GetShortcutsHtml();
                    break;
                case "faq":
                    body = GetFaqHtml();
                    break;
                default:
                    body = GetAboutHtml();
                    break;
            }

            return "<!DOCTYPE html><html><head><meta charset='utf-8'/><style>"
                + "body{font-family:'Microsoft YaHei UI','Segoe UI',sans-serif;font-size:14px;color:#333;margin:24px 28px;line-height:1.7;}"
                + "h1{font-size:22px;color:#333;margin-bottom:4px;}"
                + "h2{font-size:16px;color:#68217A;margin-top:20px;border-bottom:none;}"
                + "h3{font-size:14px;color:#68217A;margin-top:14px;}"
                + "p{margin:8px 0;}"
                + "ul{padding-left:20px;}"
                + "li{margin:4px 0;}"
                + "code{background:#f5f5f5;padding:2px 5px;border-radius:3px;font-family:Consolas,'Courier New',monospace;font-size:13px;}"
                + "table{border-collapse:collapse;margin:12px 0;}"
                + "th,td{border:1px solid #ddd;padding:6px 12px;text-align:left;}"
                + "th{background:#f0f0f0;}"
                + ".version{color:#666;font-size:13px;margin-bottom:16px;}"
                + ".highlight{color:#68217A;}"
                + "</style></head><body>" + body + "</body></html>";
        }

        private static string GetAboutHtml()
        {
            return "<h1>关于 OneNote Markdown</h1>"
                + "<p class='version'>版本: 1.1.0</p>"
                + "<h2>开发背景</h2>"
                + "<p>本插件由 OneNote MVP 开发。</p>"
                + "<p>在 Microsoft OneNote 中编写和渲染 Markdown 的插件。支持实时预览、LaTeX 公式、"
                + "图表源码块和代码高亮，为 OneNote 用户提供流畅的 Markdown 写作体验。</p>"
                + "<h2>主要功能</h2>"
                + "<ul>"
                + "<li>Markdown 渲染 —— 支持标题、列表、代码块、表格等</li>"
                + "<li>实时预览模式 —— 编辑后自动刷新渲染</li>"
                + "<li>LaTeX 公式 —— 行内和块级数学公式</li>"
                + "<li>图表源码块 —— Mermaid 等图表以保留格式显示</li>"
                + "<li>代码高亮 —— 多种语言语法着色</li>"
                + "<li>导入/导出 —— .md 文件导入渲染，页面导出为 Markdown</li>"
                + "</ul>"
                + "<h2>许可证</h2>"
                + "<p>MIT License</p>";
        }

        private static string GetQuickStartHtml()
        {
            return "<h1>快速入门</h1>"
                + "<h2>安装</h2>"
                + "<ol>"
                + "<li>确认 OneNote 位数：<code>文件 → 帐户 → 关于 OneNote</code></li>"
                + "<li>32 位 OneNote 安装 <code>OneNoteMarkdownSetup-x86.exe</code></li>"
                + "<li>64 位 OneNote 安装 <code>OneNoteMarkdownSetup-x64.exe</code></li>"
                + "<li>安装后，功能区出现 \"Markdown\" 选项卡</li>"
                + "</ol>"
                + "<p><b>注意：</b>安装包必须匹配 <b>OneNote 的位数</b>，不是 Windows 的位数。</p>"
                + "<h2>第一次使用</h2>"
                + "<ol>"
                + "<li>在 OneNote 页面中输入 Markdown 文本</li>"
                + "<li>点击功能区的 \"渲染整页\" 或按 <code>F5</code></li>"
                + "<li>页面末尾将生成渲染后的格式化内容</li>"
                + "</ol>";
        }

        private static string GetFeaturesOverviewHtml()
        {
            return "<h1>功能说明</h1>"
                + "<p>OneNote Markdown 提供以下功能，请从左侧目录选择查看详情：</p>"
                + "<ul>"
                + "<li><b>渲染整页</b> —— 将当前页正文按 Markdown 重新渲染</li>"
                + "<li><b>渲染选区</b> —— 渲染选中的 Markdown 文本</li>"
                + "<li><b>实时模式</b> —— 源码/预览双区实时同步</li>"
                + "<li><b>导入/导出</b> —— .md 文件与 OneNote 页面互转</li>"
                + "<li><b>LaTeX 公式</b> —— 行内与块级数学公式渲染</li>"
                + "<li><b>代码高亮</b> —— 多语言语法着色</li>"
                + "</ul>";
        }

        private static string GetRenderPageHtml()
        {
            return "<h1>渲染整页</h1>"
                + "<p>将当前页面的全部正文内容按 Markdown 语法解析，并在页面末尾生成渲染后的格式化内容。</p>"
                + "<h2>使用方法</h2>"
                + "<ul>"
                + "<li>功能区点击 \"渲染整页\"</li>"
                + "<li>或按快捷键 <code>F5</code></li>"
                + "</ul>"
                + "<h2>支持的语法</h2>"
                + "<ul>"
                + "<li>标题：<code>#</code> 到 <code>######</code></li>"
                + "<li>无序列表：<code>-</code> <code>*</code> <code>+</code></li>"
                + "<li>有序列表：<code>1.</code> / <code>1)</code></li>"
                + "<li>任务列表：<code>- [ ]</code> / <code>- [x]</code></li>"
                + "<li>代码块：<code>```</code> 和 <code>~~~</code></li>"
                + "<li>行内强调：<code>**粗体**</code> <code>*斜体*</code> <code>`代码`</code> <code>~~删除~~</code></li>"
                + "<li>TOC：<code>[TOC]</code> / <code>[[_TOC_]]</code> / <code>{:toc}</code></li>"
                + "<li>图表块：mermaid / mindmap / flow / sequence</li>"
                + "<li>LaTeX 块：<code>$$ ... $$</code></li>"
                + "</ul>";
        }

        private static string GetRenderSelHtml()
        {
            return "<h1>渲染选区</h1>"
                + "<p>仅渲染当前选中的 Markdown 文本，结果追加到页面末尾。</p>"
                + "<h2>使用方法</h2>"
                + "<ol>"
                + "<li>在 OneNote 中选中一段 Markdown 文本</li>"
                + "<li>点击功能区 \"渲染选区\"</li>"
                + "</ol>"
                + "<p>适合只渲染页面中的部分内容，而不影响整页。</p>";
        }

        private static string GetLiveHtml()
        {
            return "<h1>实时模式</h1>"
                + "<p>开启后，页面会自动创建两块区域：</p>"
                + "<ul>"
                + "<li><b>Markdown 源码（实时）</b> —— 在此编辑 Markdown</li>"
                + "<li><b>Markdown 预览（实时）</b> —— 自动刷新渲染结果</li>"
                + "</ul>"
                + "<h2>使用方法</h2>"
                + "<ol>"
                + "<li>点击功能区 \"实时模式\" 或按 <code>Ctrl+\\</code></li>"
                + "<li>在 \"Markdown 源码（实时）\" 区域编写</li>"
                + "<li>回车后，预览区自动刷新</li>"
                + "</ol>"
                + "<h2>注意事项</h2>"
                + "<ul>"
                + "<li>刷新受 OneNote 编辑提交时机影响，可能有 0.2~0.5 秒延迟</li>"
                + "<li>如未自动刷新，可按 <code>F5</code> 手动刷新</li>"
                + "</ul>";
        }

        private static string GetIoHtml()
        {
            return "<h1>导入/导出</h1>"
                + "<h2>导入 Markdown</h2>"
                + "<p>将 <code>.md</code> 文件导入并渲染到当前 OneNote 页面。</p>"
                + "<ul><li>点击功能区 \"导入 Markdown\"</li><li>选择 .md 文件</li></ul>"
                + "<h2>导出 Markdown</h2>"
                + "<p>将当前页面内容导出为 <code>.md</code> 文件。</p>"
                + "<ul><li>点击功能区 \"导出 Markdown\"</li><li>选择保存位置</li></ul>"
                + "<h2>复制到剪贴板</h2>"
                + "<p>将当前页导出为 Markdown 并直接复制到剪贴板。</p>"
                + "<ul><li>点击功能区 \"复制 Markdown\" 或按 <code>F8</code></li></ul>";
        }

        private static string GetLatexHtml()
        {
            return "<h1>LaTeX 公式</h1>"
                + "<p>支持 LaTeX 数学公式渲染为图片插入 OneNote。</p>"
                + "<h2>块级公式</h2>"
                + "<p>使用 <code>$$</code> 包裹：</p>"
                + "<pre><code>$$\nE = mc^2\n$$</code></pre>"
                + "<h2>配置</h2>"
                + "<p>在 <code>theme.ini</code> 中可设置：</p>"
                + "<ul>"
                + "<li><code>enable.latex.image=true</code> —— 开启/关闭 LaTeX 图片渲染</li>"
                + "<li><code>font.math=Cambria Math</code> —— 数学字体</li>"
                + "</ul>";
        }

        private static string GetCodeHtml()
        {
            return "<h1>代码高亮</h1>"
                + "<p>围栏代码块支持多种语言的语法高亮。</p>"
                + "<h2>使用方法</h2>"
                + "<pre><code>```csharp\npublic class Hello {\n    Console.WriteLine(\"Hi\");\n}\n```</code></pre>"
                + "<h2>配置</h2>"
                + "<ul>"
                + "<li><code>font.monospace=Consolas</code> —— 代码字体</li>"
                + "<li><code>font.size.code=10</code> —— 代码字号</li>"
                + "<li><code>enable.code.lineNumber=false</code> —— 是否显示行号</li>"
                + "</ul>";
        }

        private static string GetSettingsHtml()
        {
            return "<h1>设置说明</h1>"
                + "<p>设置文件位于：</p>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\settings\\theme.ini</code></p>"
                + "<p>点击功能区 \"设置\" 按钮可直接打开编辑。</p>"
                + "<h2>可配置项</h2>"
                + "<table>"
                + "<tr><th>配置项</th><th>默认值</th><th>说明</th></tr>"
                + "<tr><td><code>font.family</code></td><td>Calibri</td><td>正文字体</td></tr>"
                + "<tr><td><code>font.monospace</code></td><td>Consolas</td><td>代码字体</td></tr>"
                + "<tr><td><code>font.math</code></td><td>Cambria Math</td><td>数学公式字体</td></tr>"
                + "<tr><td><code>font.size.paragraph</code></td><td>11</td><td>正文字号</td></tr>"
                + "<tr><td><code>font.size.code</code></td><td>10</td><td>代码字号</td></tr>"
                + "<tr><td><code>enable.latex.image</code></td><td>true</td><td>是否渲染 LaTeX 为图片</td></tr>"
                + "<tr><td><code>enable.code.lineNumber</code></td><td>false</td><td>是否显示代码行号</td></tr>"
                + "</table>"
                + "<p>修改后重新渲染页面即可生效。</p>";
        }

        private static string GetShortcutsHtml()
        {
            return "<h1>快捷键</h1>"
                + "<table>"
                + "<tr><th>快捷键</th><th>功能</th></tr>"
                + "<tr><td><code>F5</code></td><td>渲染整页</td></tr>"
                + "<tr><td><code>F8</code></td><td>复制 Markdown 到剪贴板</td></tr>"
                + "<tr><td><code>Ctrl+\\</code></td><td>开启/关闭实时模式</td></tr>"
                + "<tr><td><code>Ctrl+Enter</code></td><td>渲染当前行</td></tr>"
                + "</table>"
                + "<h2>说明</h2>"
                + "<ul>"
                + "<li>快捷键为全局注册，仅在 OneNote 窗口激活时触发</li>"
                + "<li>当前版本不再拦截 Enter 键，避免影响正常换行输入</li>"
                + "</ul>";
        }

        private static string GetFaqHtml()
        {
            return "<h1>常见问题</h1>"
                + "<h2>插件未加载</h2>"
                + "<p>请确认安装包位数与 OneNote 位数一致。64 位 Windows 也可能安装了 32 位 OneNote。</p>"
                + "<h2>渲染后没有变化</h2>"
                + "<ul>"
                + "<li>确认页面中确实包含 Markdown 语法内容</li>"
                + "<li>查看日志文件排查错误</li>"
                + "</ul>"
                + "<h2>实时模式延迟</h2>"
                + "<p>受 OneNote 编辑提交时机影响，可能有 0.2~0.5 秒延迟。如仍未变化，按 <code>F5</code> 手动刷新。</p>"
                + "<h2>日志文件位置</h2>"
                + "<p><code>%AppData%\\OneNoteMarkdown\\logs\\onenotemarkdown.log</code></p>"
                + "<h2>目前限制</h2>"
                + "<ul>"
                + "<li>实时模式非完全逐行即时体验</li>"
                + "<li>图表与 LaTeX 以安全兼容优先</li>"
                + "<li>较旧 OneNote 版本渲染表现可能有差异</li>"
                + "</ul>";
        }
    }
}
