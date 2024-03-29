﻿namespace DirectX11TutorialLevelEditor
{
    partial class MainFrm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.SplitTab = new System.Windows.Forms.SplitContainer();
            this.TabTileView = new System.Windows.Forms.TabControl();
            this.TabDesign = new System.Windows.Forms.TabPage();
            this.TabMovement = new System.Windows.Forms.TabPage();
            this.TabObjectSet = new System.Windows.Forms.TabPage();
            this.LabelLevelSize = new System.Windows.Forms.Label();
            this.LabelLevelName = new System.Windows.Forms.Label();
            this.LabelScale = new System.Windows.Forms.Label();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.맵ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.새로만들기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.불러오기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.저장하기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.타일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.불러오기ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.크기설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.오브젝트ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.오브젝트셋지정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SplitViews = new System.Windows.Forms.SplitContainer();
            this.SplitObjectView = new System.Windows.Forms.SplitContainer();
            this.SplitObjectSet = new System.Windows.Forms.SplitContainer();
            this.lbObjectSet = new System.Windows.Forms.ListBox();
            this.TileViewHScrollBar = new System.Windows.Forms.HScrollBar();
            this.TileViewVScrollBar = new System.Windows.Forms.VScrollBar();
            this.LevelViewHScrollBar = new System.Windows.Forms.HScrollBar();
            this.LevelViewVScrollBar = new System.Windows.Forms.VScrollBar();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SplitInsertedObjects = new System.Windows.Forms.SplitContainer();
            this.lbInsertedObjests = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTab)).BeginInit();
            this.SplitTab.Panel1.SuspendLayout();
            this.SplitTab.Panel2.SuspendLayout();
            this.SplitTab.SuspendLayout();
            this.TabTileView.SuspendLayout();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitViews)).BeginInit();
            this.SplitViews.Panel1.SuspendLayout();
            this.SplitViews.Panel2.SuspendLayout();
            this.SplitViews.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitObjectView)).BeginInit();
            this.SplitObjectView.Panel1.SuspendLayout();
            this.SplitObjectView.Panel2.SuspendLayout();
            this.SplitObjectView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitObjectSet)).BeginInit();
            this.SplitObjectSet.Panel2.SuspendLayout();
            this.SplitObjectSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitInsertedObjects)).BeginInit();
            this.SplitInsertedObjects.Panel1.SuspendLayout();
            this.SplitInsertedObjects.Panel2.SuspendLayout();
            this.SplitInsertedObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitTab
            // 
            this.SplitTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.SplitTab.Location = new System.Drawing.Point(0, 24);
            this.SplitTab.Name = "SplitTab";
            // 
            // SplitTab.Panel1
            // 
            this.SplitTab.Panel1.Controls.Add(this.TabTileView);
            // 
            // SplitTab.Panel2
            // 
            this.SplitTab.Panel2.Controls.Add(this.LabelLevelSize);
            this.SplitTab.Panel2.Controls.Add(this.LabelLevelName);
            this.SplitTab.Panel2.Controls.Add(this.LabelScale);
            this.SplitTab.Size = new System.Drawing.Size(800, 21);
            this.SplitTab.SplitterDistance = 266;
            this.SplitTab.TabIndex = 0;
            // 
            // TabTileView
            // 
            this.TabTileView.Controls.Add(this.TabDesign);
            this.TabTileView.Controls.Add(this.TabMovement);
            this.TabTileView.Controls.Add(this.TabObjectSet);
            this.TabTileView.Dock = System.Windows.Forms.DockStyle.Top;
            this.TabTileView.Location = new System.Drawing.Point(0, 0);
            this.TabTileView.Name = "TabTileView";
            this.TabTileView.SelectedIndex = 0;
            this.TabTileView.Size = new System.Drawing.Size(266, 21);
            this.TabTileView.TabIndex = 2;
            this.TabTileView.SelectedIndexChanged += new System.EventHandler(this.TabTileView_SelectedIndexChanged);
            // 
            // TabDesign
            // 
            this.TabDesign.Location = new System.Drawing.Point(4, 22);
            this.TabDesign.Name = "TabDesign";
            this.TabDesign.Padding = new System.Windows.Forms.Padding(3);
            this.TabDesign.Size = new System.Drawing.Size(258, 0);
            this.TabDesign.TabIndex = 0;
            this.TabDesign.Text = "디자인 타일";
            this.TabDesign.UseVisualStyleBackColor = true;
            // 
            // TabMovement
            // 
            this.TabMovement.Location = new System.Drawing.Point(4, 22);
            this.TabMovement.Name = "TabMovement";
            this.TabMovement.Padding = new System.Windows.Forms.Padding(3);
            this.TabMovement.Size = new System.Drawing.Size(258, 0);
            this.TabMovement.TabIndex = 1;
            this.TabMovement.Text = "움직임 타일";
            this.TabMovement.UseVisualStyleBackColor = true;
            // 
            // TabObjectSet
            // 
            this.TabObjectSet.Location = new System.Drawing.Point(4, 22);
            this.TabObjectSet.Name = "TabObjectSet";
            this.TabObjectSet.Padding = new System.Windows.Forms.Padding(3);
            this.TabObjectSet.Size = new System.Drawing.Size(258, 0);
            this.TabObjectSet.TabIndex = 2;
            this.TabObjectSet.Text = "오브젝트";
            this.TabObjectSet.UseVisualStyleBackColor = true;
            // 
            // LabelLevelSize
            // 
            this.LabelLevelSize.Location = new System.Drawing.Point(402, 6);
            this.LabelLevelSize.Name = "LabelLevelSize";
            this.LabelLevelSize.Size = new System.Drawing.Size(122, 12);
            this.LabelLevelSize.TabIndex = 2;
            this.LabelLevelSize.Text = "레벨 크기: 5 x 3";
            this.LabelLevelSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // LabelLevelName
            // 
            this.LabelLevelName.AutoSize = true;
            this.LabelLevelName.Location = new System.Drawing.Point(90, 6);
            this.LabelLevelName.Name = "LabelLevelName";
            this.LabelLevelName.Size = new System.Drawing.Size(121, 12);
            this.LabelLevelName.TabIndex = 1;
            this.LabelLevelName.Text = "레벨 이름: level_new";
            // 
            // LabelScale
            // 
            this.LabelScale.AutoSize = true;
            this.LabelScale.Location = new System.Drawing.Point(3, 6);
            this.LabelScale.Name = "LabelScale";
            this.LabelScale.Size = new System.Drawing.Size(69, 12);
            this.LabelScale.TabIndex = 0;
            this.LabelScale.Text = "배율: 100 %";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.맵ToolStripMenuItem,
            this.타일ToolStripMenuItem,
            this.오브젝트ToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(800, 24);
            this.MainMenu.TabIndex = 2;
            this.MainMenu.Text = "menuStrip1";
            // 
            // 맵ToolStripMenuItem
            // 
            this.맵ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.새로만들기ToolStripMenuItem,
            this.불러오기ToolStripMenuItem,
            this.저장하기ToolStripMenuItem});
            this.맵ToolStripMenuItem.Name = "맵ToolStripMenuItem";
            this.맵ToolStripMenuItem.Size = new System.Drawing.Size(31, 20);
            this.맵ToolStripMenuItem.Text = "맵";
            // 
            // 새로만들기ToolStripMenuItem
            // 
            this.새로만들기ToolStripMenuItem.Name = "새로만들기ToolStripMenuItem";
            this.새로만들기ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.새로만들기ToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.새로만들기ToolStripMenuItem.Text = "새로만들기";
            this.새로만들기ToolStripMenuItem.Click += new System.EventHandler(this.새로만들기ToolStripMenuItem_Click);
            // 
            // 불러오기ToolStripMenuItem
            // 
            this.불러오기ToolStripMenuItem.Name = "불러오기ToolStripMenuItem";
            this.불러오기ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.불러오기ToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.불러오기ToolStripMenuItem.Text = "불러오기";
            this.불러오기ToolStripMenuItem.Click += new System.EventHandler(this.불러오기ToolStripMenuItem_Click);
            // 
            // 저장하기ToolStripMenuItem
            // 
            this.저장하기ToolStripMenuItem.Name = "저장하기ToolStripMenuItem";
            this.저장하기ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.저장하기ToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.저장하기ToolStripMenuItem.Text = "저장하기";
            this.저장하기ToolStripMenuItem.Click += new System.EventHandler(this.저장하기ToolStripMenuItem_Click);
            // 
            // 타일ToolStripMenuItem
            // 
            this.타일ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.불러오기ToolStripMenuItem1,
            this.크기설정ToolStripMenuItem});
            this.타일ToolStripMenuItem.Name = "타일ToolStripMenuItem";
            this.타일ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.타일ToolStripMenuItem.Text = "타일";
            // 
            // 불러오기ToolStripMenuItem1
            // 
            this.불러오기ToolStripMenuItem1.Name = "불러오기ToolStripMenuItem1";
            this.불러오기ToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.불러오기ToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.불러오기ToolStripMenuItem1.Text = "불러오기";
            this.불러오기ToolStripMenuItem1.Click += new System.EventHandler(this.타일불러오기ToolStripMenuItem_Click);
            // 
            // 크기설정ToolStripMenuItem
            // 
            this.크기설정ToolStripMenuItem.Name = "크기설정ToolStripMenuItem";
            this.크기설정ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
            this.크기설정ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.크기설정ToolStripMenuItem.Text = "크기 설정";
            this.크기설정ToolStripMenuItem.Click += new System.EventHandler(this.타일크기설정ToolStripMenuItem_Click);
            // 
            // 오브젝트ToolStripMenuItem
            // 
            this.오브젝트ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.오브젝트셋지정ToolStripMenuItem});
            this.오브젝트ToolStripMenuItem.Name = "오브젝트ToolStripMenuItem";
            this.오브젝트ToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.오브젝트ToolStripMenuItem.Text = "오브젝트";
            // 
            // 오브젝트셋지정ToolStripMenuItem
            // 
            this.오브젝트셋지정ToolStripMenuItem.Name = "오브젝트셋지정ToolStripMenuItem";
            this.오브젝트셋지정ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.오브젝트셋지정ToolStripMenuItem.Text = "오브젝트셋 지정";
            this.오브젝트셋지정ToolStripMenuItem.Click += new System.EventHandler(this.오브젝트셋지정ToolStripMenuItem_Click);
            // 
            // SplitViews
            // 
            this.SplitViews.Dock = System.Windows.Forms.DockStyle.Top;
            this.SplitViews.Location = new System.Drawing.Point(0, 45);
            this.SplitViews.Name = "SplitViews";
            // 
            // SplitViews.Panel1
            // 
            this.SplitViews.Panel1.Controls.Add(this.SplitObjectView);
            this.SplitViews.Panel1.Controls.Add(this.TileViewHScrollBar);
            this.SplitViews.Panel1.Controls.Add(this.TileViewVScrollBar);
            this.SplitViews.Panel1.Resize += new System.EventHandler(this.SplitViews_Panel1_Resize);
            // 
            // SplitViews.Panel2
            // 
            this.SplitViews.Panel2.Controls.Add(this.LevelViewHScrollBar);
            this.SplitViews.Panel2.Controls.Add(this.LevelViewVScrollBar);
            this.SplitViews.Panel2.Resize += new System.EventHandler(this.SplitViews_Panel2_Resize);
            this.SplitViews.Size = new System.Drawing.Size(800, 359);
            this.SplitViews.SplitterDistance = 266;
            this.SplitViews.TabIndex = 3;
            // 
            // SplitObjectView
            // 
            this.SplitObjectView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitObjectView.Location = new System.Drawing.Point(0, 0);
            this.SplitObjectView.Name = "SplitObjectView";
            this.SplitObjectView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitObjectView.Panel1
            // 
            this.SplitObjectView.Panel1.Controls.Add(this.SplitInsertedObjects);
            // 
            // SplitObjectView.Panel2
            // 
            this.SplitObjectView.Panel2.Controls.Add(this.SplitObjectSet);
            this.SplitObjectView.Size = new System.Drawing.Size(249, 342);
            this.SplitObjectView.SplitterDistance = 121;
            this.SplitObjectView.TabIndex = 5;
            // 
            // SplitObjectSet
            // 
            this.SplitObjectSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitObjectSet.Location = new System.Drawing.Point(0, 0);
            this.SplitObjectSet.Name = "SplitObjectSet";
            this.SplitObjectSet.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitObjectSet.Panel2
            // 
            this.SplitObjectSet.Panel2.Controls.Add(this.lbObjectSet);
            this.SplitObjectSet.Size = new System.Drawing.Size(249, 217);
            this.SplitObjectSet.SplitterDistance = 83;
            this.SplitObjectSet.TabIndex = 1;
            // 
            // lbObjectSet
            // 
            this.lbObjectSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbObjectSet.FormattingEnabled = true;
            this.lbObjectSet.ItemHeight = 12;
            this.lbObjectSet.Location = new System.Drawing.Point(0, 0);
            this.lbObjectSet.Name = "lbObjectSet";
            this.lbObjectSet.Size = new System.Drawing.Size(249, 130);
            this.lbObjectSet.TabIndex = 1;
            this.lbObjectSet.SelectedIndexChanged += new System.EventHandler(this.LbObjectSet_SelectedIndexChanged);
            // 
            // TileViewHScrollBar
            // 
            this.TileViewHScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TileViewHScrollBar.Location = new System.Drawing.Point(0, 342);
            this.TileViewHScrollBar.Name = "TileViewHScrollBar";
            this.TileViewHScrollBar.Size = new System.Drawing.Size(249, 17);
            this.TileViewHScrollBar.TabIndex = 1;
            this.TileViewHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.TileViewHScrollBar_Scroll);
            // 
            // TileViewVScrollBar
            // 
            this.TileViewVScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.TileViewVScrollBar.Location = new System.Drawing.Point(249, 0);
            this.TileViewVScrollBar.Name = "TileViewVScrollBar";
            this.TileViewVScrollBar.Size = new System.Drawing.Size(17, 359);
            this.TileViewVScrollBar.TabIndex = 0;
            this.TileViewVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.TileViewVScrollBar_Scroll);
            // 
            // LevelViewHScrollBar
            // 
            this.LevelViewHScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LevelViewHScrollBar.Location = new System.Drawing.Point(0, 342);
            this.LevelViewHScrollBar.Name = "LevelViewHScrollBar";
            this.LevelViewHScrollBar.Size = new System.Drawing.Size(513, 17);
            this.LevelViewHScrollBar.TabIndex = 2;
            this.LevelViewHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.LevelViewHScrollBar_Scroll);
            // 
            // LevelViewVScrollBar
            // 
            this.LevelViewVScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.LevelViewVScrollBar.Location = new System.Drawing.Point(513, 0);
            this.LevelViewVScrollBar.Name = "LevelViewVScrollBar";
            this.LevelViewVScrollBar.Size = new System.Drawing.Size(17, 359);
            this.LevelViewVScrollBar.TabIndex = 1;
            this.LevelViewVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.LevelViewVScrollBar_Scroll);
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.RestoreDirectory = true;
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // SplitInsertedObjects
            // 
            this.SplitInsertedObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitInsertedObjects.IsSplitterFixed = true;
            this.SplitInsertedObjects.Location = new System.Drawing.Point(0, 0);
            this.SplitInsertedObjects.Name = "SplitInsertedObjects";
            this.SplitInsertedObjects.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitInsertedObjects.Panel1
            // 
            this.SplitInsertedObjects.Panel1.Controls.Add(this.label1);
            // 
            // SplitInsertedObjects.Panel2
            // 
            this.SplitInsertedObjects.Panel2.Controls.Add(this.lbInsertedObjests);
            this.SplitInsertedObjects.Size = new System.Drawing.Size(249, 121);
            this.SplitInsertedObjects.SplitterDistance = 25;
            this.SplitInsertedObjects.TabIndex = 4;
            // 
            // lbInsertedObjests
            // 
            this.lbInsertedObjests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInsertedObjests.FormattingEnabled = true;
            this.lbInsertedObjests.ItemHeight = 12;
            this.lbInsertedObjests.Location = new System.Drawing.Point(0, 0);
            this.lbInsertedObjests.Name = "lbInsertedObjests";
            this.lbInsertedObjests.Size = new System.Drawing.Size(249, 92);
            this.lbInsertedObjests.TabIndex = 2;
            this.lbInsertedObjests.SelectedIndexChanged += new System.EventHandler(this.LbInsertedObjests_SelectedIndexChanged);
            this.lbInsertedObjests.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LbInsertedObjests_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "등록된 오브젝트 목록";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SplitViews);
            this.Controls.Add(this.SplitTab);
            this.Controls.Add(this.MainMenu);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainFrm";
            this.Text = "MainFrm";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainFrm_KeyDown);
            this.Resize += new System.EventHandler(this.MainFrm_Resize);
            this.SplitTab.Panel1.ResumeLayout(false);
            this.SplitTab.Panel2.ResumeLayout(false);
            this.SplitTab.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitTab)).EndInit();
            this.SplitTab.ResumeLayout(false);
            this.TabTileView.ResumeLayout(false);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.SplitViews.Panel1.ResumeLayout(false);
            this.SplitViews.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitViews)).EndInit();
            this.SplitViews.ResumeLayout(false);
            this.SplitObjectView.Panel1.ResumeLayout(false);
            this.SplitObjectView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitObjectView)).EndInit();
            this.SplitObjectView.ResumeLayout(false);
            this.SplitObjectSet.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitObjectSet)).EndInit();
            this.SplitObjectSet.ResumeLayout(false);
            this.SplitInsertedObjects.Panel1.ResumeLayout(false);
            this.SplitInsertedObjects.Panel1.PerformLayout();
            this.SplitInsertedObjects.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitInsertedObjects)).EndInit();
            this.SplitInsertedObjects.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitTab;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem 맵ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 새로만들기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 불러오기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 저장하기ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer SplitViews;
        private System.Windows.Forms.TabControl TabTileView;
        private System.Windows.Forms.TabPage TabDesign;
        private System.Windows.Forms.TabPage TabMovement;
        private System.Windows.Forms.HScrollBar TileViewHScrollBar;
        private System.Windows.Forms.VScrollBar TileViewVScrollBar;
        private System.Windows.Forms.HScrollBar LevelViewHScrollBar;
        private System.Windows.Forms.VScrollBar LevelViewVScrollBar;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Label LabelScale;
        private System.Windows.Forms.Label LabelLevelSize;
        private System.Windows.Forms.Label LabelLevelName;
        private System.Windows.Forms.ToolStripMenuItem 타일ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 불러오기ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 크기설정ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 오브젝트ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 오브젝트셋지정ToolStripMenuItem;
        private System.Windows.Forms.TabPage TabObjectSet;
        private System.Windows.Forms.SplitContainer SplitObjectView;
        private System.Windows.Forms.SplitContainer SplitObjectSet;
        private System.Windows.Forms.ListBox lbObjectSet;
        private System.Windows.Forms.SplitContainer SplitInsertedObjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbInsertedObjests;
    }
}

