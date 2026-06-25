using System;
using System.Drawing;
using System.Windows.Forms;
using OneNoteMarkdown.Localization;

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
            Text = Loc.S("Dialog.Help.Title");
            Size = new Size(900, 640);
            MinimumSize = new Size(700, 480);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            ShowInTaskbar = true;
            TopMost = true;
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
                Text = Loc.S("Dialog.Help.Header"),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei UI", 12f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            _headerPanel.Controls.Add(headerLabel);

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
                Text = Loc.S("Common.Close"),
                Size = new Size(80, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                FlatStyle = FlatStyle.System
            };
            _closeButton.Click += delegate { Close(); };
            bottomPanel.Controls.Add(_closeButton);
            _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            bottomPanel.Resize += delegate
            {
                _closeButton.Location = new Point(bottomPanel.Width - _closeButton.Width - 16, 9);
            };

            // Left tree panel
            Panel treePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = Color.FromArgb(252, 252, 252)
            };

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
            treePanel.Controls.Add(_treeView);

            // Splitter bar
            Panel splitterBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 1,
                BackColor = Color.FromArgb(220, 220, 220)
            };

            // Right: WebBrowser
            _browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                IsWebBrowserContextMenuEnabled = false,
                ScriptErrorsSuppressed = true
            };

            // Assembly order for Dock: Fill must be added first, then non-Fill
            Controls.Add(_browser);
            Controls.Add(splitterBar);
            Controls.Add(treePanel);
            Controls.Add(bottomPanel);
            Controls.Add(_headerPanel);
            CancelButton = _closeButton;
        }

        private void PopulateTree()
        {
            TreeNode root = new TreeNode(Loc.S("Help.Tree.Root")) { Tag = "about" };

            TreeNode quickStart = new TreeNode(Loc.S("Help.Tree.QuickStart")) { Tag = "quickstart" };
            root.Nodes.Add(quickStart);

            TreeNode features = new TreeNode(Loc.S("Help.Tree.Features")) { Tag = "features" };
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.RenderPage")) { Tag = "feat_renderpage" });
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.RenderSelection")) { Tag = "feat_rendersel" });
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.LiveMode")) { Tag = "feat_live" });
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.ImportExport")) { Tag = "feat_io" });
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.LaTeX")) { Tag = "feat_latex" });
            features.Nodes.Add(new TreeNode(Loc.S("Help.Tree.CodeHighlight")) { Tag = "feat_code" });
            root.Nodes.Add(features);

            TreeNode settings = new TreeNode(Loc.S("Help.Tree.Settings")) { Tag = "settings" };
            root.Nodes.Add(settings);

            TreeNode shortcuts = new TreeNode(Loc.S("Help.Tree.Shortcuts")) { Tag = "shortcuts" };
            root.Nodes.Add(shortcuts);

            TreeNode faq = new TreeNode(Loc.S("Help.Tree.FAQ")) { Tag = "faq" };
            root.Nodes.Add(faq);

            TreeNode about = new TreeNode(Loc.S("Help.Tree.About")) { Tag = "about" };
            root.Nodes.Add(about);

            _treeView.Nodes.Add(root);
            root.ExpandAll();
            _treeView.SelectedNode = root.Nodes[root.Nodes.Count - 1]; // select "鍏充簬"
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            string tag = e.Node.Tag as string;
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
            string body = HelpContent.GetHtml(section);

            return "<!DOCTYPE html><html><head><meta charset='utf-8'/><style>"
                + "body{font-family:'Microsoft YaHei UI','Segoe UI',sans-serif;font-size:14px;color:#333;margin:24px 28px;line-height:1.7;}"
                + "h1{font-size:22px;color:#333;margin-bottom:4px;}"
                + "h2{font-size:16px;color:#68217A;margin-top:20px;border-bottom:none;}"
                + "h3{font-size:14px;color:#68217A;margin-top:14px;}"
                + "p{margin:8px 0;}"
                + "ul,ol{padding-left:20px;}"
                + "li{margin:4px 0;}"
                + "code{background:#f5f5f5;padding:2px 5px;border-radius:3px;font-family:Consolas,'Courier New',monospace;font-size:13px;}"
                + "pre{background:#f8f8f8;border:1px solid #e0e0e0;border-radius:4px;padding:12px 16px;overflow-x:auto;margin:10px 0;}"
                + "pre code{background:none;padding:0;font-size:12px;line-height:1.5;}"
                + "table{border-collapse:collapse;margin:12px 0;width:100%;}"
                + "th,td{border:1px solid #ddd;padding:6px 12px;text-align:left;}"
                + "th{background:#f0f0f0;font-weight:bold;}"
                + ".version{color:#666;font-size:13px;margin-bottom:16px;}"
                + ".highlight{color:#68217A;}"
                + "b{color:#333;}"
                + "</style></head><body>" + body + "</body></html>";
        }
    }
}
