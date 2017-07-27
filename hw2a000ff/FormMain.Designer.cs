namespace hw2a000ff
{
	partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.textOutputPrefix = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.buttonConvertUnit = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.trackActorHealthScale = new System.Windows.Forms.TrackBar();
			this.trackRangeScale = new System.Windows.Forms.TrackBar();
			this.label4 = new System.Windows.Forms.Label();
			this.trackDamageScale = new System.Windows.Forms.TrackBar();
			this.label5 = new System.Windows.Forms.Label();
			this.trackActorSpeedScale = new System.Windows.Forms.TrackBar();
			this.label6 = new System.Windows.Forms.Label();
			this.textOutputPath = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.checkModifyWallCollision = new System.Windows.Forms.CheckBox();
			this.textSourcePath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.textStringsKeyPrefix = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textSourceFallbackPath = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.buttonConvert = new System.Windows.Forms.Button();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.checkConvertActors = new System.Windows.Forms.CheckBox();
			this.checkConvertProjectiles = new System.Windows.Forms.CheckBox();
			this.checkConvertDoodads = new System.Windows.Forms.CheckBox();
			this.checkConvertTilesets = new System.Windows.Forms.CheckBox();
			this.checkConvertItems = new System.Windows.Forms.CheckBox();
			this.checkConvertStrings = new System.Windows.Forms.CheckBox();
			this.checkConvertSpeechStyles = new System.Windows.Forms.CheckBox();
			this.checkConvertFonts = new System.Windows.Forms.CheckBox();
			this.checkConvertLoot = new System.Windows.Forms.CheckBox();
			this.checkConvertLevels = new System.Windows.Forms.CheckBox();
			this.textLevelsXmlPath = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.buttonBrowseLevelsXml = new System.Windows.Forms.Button();
			this.checkConvertSounds = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackActorHealthScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackRangeScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackDamageScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackActorSpeedScale)).BeginInit();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(450, 320);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.buttonBrowseLevelsXml);
			this.tabPage1.Controls.Add(this.textLevelsXmlPath);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.buttonConvert);
			this.tabPage1.Controls.Add(this.label10);
			this.tabPage1.Controls.Add(this.textSourceFallbackPath);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this.textSourcePath);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.textOutputPath);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.textOutputPrefix);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(442, 294);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Main";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// textOutputPrefix
			// 
			this.textOutputPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textOutputPrefix.Location = new System.Drawing.Point(106, 110);
			this.textOutputPrefix.Name = "textOutputPrefix";
			this.textOutputPrefix.Size = new System.Drawing.Size(330, 20);
			this.textOutputPrefix.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Output path prefix:";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.trackActorSpeedScale);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.trackDamageScale);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this.trackRangeScale);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.trackActorHealthScale);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.buttonConvertUnit);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(442, 294);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Units";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// buttonConvertUnit
			// 
			this.buttonConvertUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonConvertUnit.Location = new System.Drawing.Point(299, 265);
			this.buttonConvertUnit.Name = "buttonConvertUnit";
			this.buttonConvertUnit.Size = new System.Drawing.Size(137, 23);
			this.buttonConvertUnit.TabIndex = 0;
			this.buttonConvertUnit.Text = "Convert single unit";
			this.buttonConvertUnit.UseVisualStyleBackColor = true;
			this.buttonConvertUnit.Click += new System.EventHandler(this.buttonConvertUnit_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(26, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Actor health scale:";
			// 
			// trackActorHealthScale
			// 
			this.trackActorHealthScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackActorHealthScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.trackActorHealthScale.Location = new System.Drawing.Point(127, 6);
			this.trackActorHealthScale.Maximum = 1000;
			this.trackActorHealthScale.Minimum = 1;
			this.trackActorHealthScale.Name = "trackActorHealthScale";
			this.trackActorHealthScale.Size = new System.Drawing.Size(309, 45);
			this.trackActorHealthScale.TabIndex = 5;
			this.trackActorHealthScale.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackActorHealthScale.Value = 100;
			// 
			// trackRangeScale
			// 
			this.trackRangeScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackRangeScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.trackRangeScale.Location = new System.Drawing.Point(127, 32);
			this.trackRangeScale.Maximum = 1000;
			this.trackRangeScale.Minimum = 1;
			this.trackRangeScale.Name = "trackRangeScale";
			this.trackRangeScale.Size = new System.Drawing.Size(309, 45);
			this.trackRangeScale.TabIndex = 7;
			this.trackRangeScale.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackRangeScale.Value = 100;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(51, 32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Range scale:";
			// 
			// trackDamageScale
			// 
			this.trackDamageScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackDamageScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.trackDamageScale.Location = new System.Drawing.Point(127, 58);
			this.trackDamageScale.Maximum = 1000;
			this.trackDamageScale.Minimum = 1;
			this.trackDamageScale.Name = "trackDamageScale";
			this.trackDamageScale.Size = new System.Drawing.Size(309, 45);
			this.trackDamageScale.TabIndex = 9;
			this.trackDamageScale.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackDamageScale.Value = 100;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(43, 58);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Damage scale:";
			// 
			// trackActorSpeedScale
			// 
			this.trackActorSpeedScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackActorSpeedScale.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.trackActorSpeedScale.Location = new System.Drawing.Point(127, 84);
			this.trackActorSpeedScale.Maximum = 1000;
			this.trackActorSpeedScale.Minimum = 1;
			this.trackActorSpeedScale.Name = "trackActorSpeedScale";
			this.trackActorSpeedScale.Size = new System.Drawing.Size(309, 45);
			this.trackActorSpeedScale.TabIndex = 11;
			this.trackActorSpeedScale.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackActorSpeedScale.Value = 100;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(26, 84);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(95, 13);
			this.label6.TabIndex = 10;
			this.label6.Text = "Actor speed scale:";
			// 
			// textOutputPath
			// 
			this.textOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textOutputPath.Location = new System.Drawing.Point(106, 84);
			this.textOutputPath.Name = "textOutputPath";
			this.textOutputPath.Size = new System.Drawing.Size(330, 20);
			this.textOutputPath.TabIndex = 15;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(34, 87);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(66, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Output path:";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.checkModifyWallCollision);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(442, 294);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Sprites";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// checkModifyWallCollision
			// 
			this.checkModifyWallCollision.AutoSize = true;
			this.checkModifyWallCollision.Location = new System.Drawing.Point(6, 6);
			this.checkModifyWallCollision.Name = "checkModifyWallCollision";
			this.checkModifyWallCollision.Size = new System.Drawing.Size(118, 17);
			this.checkModifyWallCollision.TabIndex = 3;
			this.checkModifyWallCollision.Text = "Modify wall collision";
			this.checkModifyWallCollision.UseVisualStyleBackColor = true;
			// 
			// textSourcePath
			// 
			this.textSourcePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSourcePath.Location = new System.Drawing.Point(106, 6);
			this.textSourcePath.Name = "textSourcePath";
			this.textSourcePath.Size = new System.Drawing.Size(330, 20);
			this.textSourcePath.TabIndex = 17;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Source path:";
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.textStringsKeyPrefix);
			this.tabPage4.Controls.Add(this.label8);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(442, 294);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Strings";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// textStringsKeyPrefix
			// 
			this.textStringsKeyPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textStringsKeyPrefix.Location = new System.Drawing.Point(106, 6);
			this.textStringsKeyPrefix.Name = "textStringsKeyPrefix";
			this.textStringsKeyPrefix.Size = new System.Drawing.Size(330, 20);
			this.textStringsKeyPrefix.TabIndex = 3;
			this.textStringsKeyPrefix.Text = "hwport.";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(44, 9);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(56, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "Key prefix:";
			// 
			// textSourceFallbackPath
			// 
			this.textSourceFallbackPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSourceFallbackPath.Location = new System.Drawing.Point(106, 32);
			this.textSourceFallbackPath.Name = "textSourceFallbackPath";
			this.textSourceFallbackPath.Size = new System.Drawing.Size(330, 20);
			this.textSourceFallbackPath.TabIndex = 19;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(26, 35);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(74, 13);
			this.label9.TabIndex = 18;
			this.label9.Text = "Fallback path:";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.BackColor = System.Drawing.SystemColors.Control;
			this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label10.Location = new System.Drawing.Point(6, 168);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(430, 123);
			this.label10.TabIndex = 20;
			this.label10.Text = resources.GetString("label10.Text");
			// 
			// buttonConvert
			// 
			this.buttonConvert.Location = new System.Drawing.Point(361, 142);
			this.buttonConvert.Name = "buttonConvert";
			this.buttonConvert.Size = new System.Drawing.Size(75, 23);
			this.buttonConvert.TabIndex = 21;
			this.buttonConvert.Text = "Convert";
			this.buttonConvert.UseVisualStyleBackColor = true;
			this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.checkConvertSounds);
			this.tabPage5.Controls.Add(this.checkConvertLevels);
			this.tabPage5.Controls.Add(this.checkConvertLoot);
			this.tabPage5.Controls.Add(this.checkConvertFonts);
			this.tabPage5.Controls.Add(this.checkConvertSpeechStyles);
			this.tabPage5.Controls.Add(this.checkConvertStrings);
			this.tabPage5.Controls.Add(this.checkConvertItems);
			this.tabPage5.Controls.Add(this.checkConvertTilesets);
			this.tabPage5.Controls.Add(this.checkConvertDoodads);
			this.tabPage5.Controls.Add(this.checkConvertProjectiles);
			this.tabPage5.Controls.Add(this.checkConvertActors);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(442, 294);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Converters";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// checkConvertActors
			// 
			this.checkConvertActors.AutoSize = true;
			this.checkConvertActors.Checked = true;
			this.checkConvertActors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertActors.Location = new System.Drawing.Point(6, 6);
			this.checkConvertActors.Name = "checkConvertActors";
			this.checkConvertActors.Size = new System.Drawing.Size(56, 17);
			this.checkConvertActors.TabIndex = 0;
			this.checkConvertActors.Text = "Actors";
			this.checkConvertActors.UseVisualStyleBackColor = true;
			// 
			// checkConvertProjectiles
			// 
			this.checkConvertProjectiles.AutoSize = true;
			this.checkConvertProjectiles.Checked = true;
			this.checkConvertProjectiles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertProjectiles.Location = new System.Drawing.Point(6, 29);
			this.checkConvertProjectiles.Name = "checkConvertProjectiles";
			this.checkConvertProjectiles.Size = new System.Drawing.Size(74, 17);
			this.checkConvertProjectiles.TabIndex = 1;
			this.checkConvertProjectiles.Text = "Projectiles";
			this.checkConvertProjectiles.UseVisualStyleBackColor = true;
			// 
			// checkConvertDoodads
			// 
			this.checkConvertDoodads.AutoSize = true;
			this.checkConvertDoodads.Checked = true;
			this.checkConvertDoodads.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertDoodads.Location = new System.Drawing.Point(6, 52);
			this.checkConvertDoodads.Name = "checkConvertDoodads";
			this.checkConvertDoodads.Size = new System.Drawing.Size(69, 17);
			this.checkConvertDoodads.TabIndex = 2;
			this.checkConvertDoodads.Text = "Doodads";
			this.checkConvertDoodads.UseVisualStyleBackColor = true;
			// 
			// checkConvertTilesets
			// 
			this.checkConvertTilesets.AutoSize = true;
			this.checkConvertTilesets.Checked = true;
			this.checkConvertTilesets.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertTilesets.Location = new System.Drawing.Point(6, 75);
			this.checkConvertTilesets.Name = "checkConvertTilesets";
			this.checkConvertTilesets.Size = new System.Drawing.Size(62, 17);
			this.checkConvertTilesets.TabIndex = 3;
			this.checkConvertTilesets.Text = "Tilesets";
			this.checkConvertTilesets.UseVisualStyleBackColor = true;
			// 
			// checkConvertItems
			// 
			this.checkConvertItems.AutoSize = true;
			this.checkConvertItems.Checked = true;
			this.checkConvertItems.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertItems.Location = new System.Drawing.Point(6, 98);
			this.checkConvertItems.Name = "checkConvertItems";
			this.checkConvertItems.Size = new System.Drawing.Size(51, 17);
			this.checkConvertItems.TabIndex = 4;
			this.checkConvertItems.Text = "Items";
			this.checkConvertItems.UseVisualStyleBackColor = true;
			// 
			// checkConvertStrings
			// 
			this.checkConvertStrings.AutoSize = true;
			this.checkConvertStrings.Checked = true;
			this.checkConvertStrings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertStrings.Location = new System.Drawing.Point(6, 121);
			this.checkConvertStrings.Name = "checkConvertStrings";
			this.checkConvertStrings.Size = new System.Drawing.Size(58, 17);
			this.checkConvertStrings.TabIndex = 5;
			this.checkConvertStrings.Text = "Strings";
			this.checkConvertStrings.UseVisualStyleBackColor = true;
			// 
			// checkConvertSpeechStyles
			// 
			this.checkConvertSpeechStyles.AutoSize = true;
			this.checkConvertSpeechStyles.Checked = true;
			this.checkConvertSpeechStyles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertSpeechStyles.Location = new System.Drawing.Point(6, 144);
			this.checkConvertSpeechStyles.Name = "checkConvertSpeechStyles";
			this.checkConvertSpeechStyles.Size = new System.Drawing.Size(92, 17);
			this.checkConvertSpeechStyles.TabIndex = 6;
			this.checkConvertSpeechStyles.Text = "Speech styles";
			this.checkConvertSpeechStyles.UseVisualStyleBackColor = true;
			// 
			// checkConvertFonts
			// 
			this.checkConvertFonts.AutoSize = true;
			this.checkConvertFonts.Checked = true;
			this.checkConvertFonts.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertFonts.Location = new System.Drawing.Point(6, 167);
			this.checkConvertFonts.Name = "checkConvertFonts";
			this.checkConvertFonts.Size = new System.Drawing.Size(52, 17);
			this.checkConvertFonts.TabIndex = 7;
			this.checkConvertFonts.Text = "Fonts";
			this.checkConvertFonts.UseVisualStyleBackColor = true;
			// 
			// checkConvertLoot
			// 
			this.checkConvertLoot.AutoSize = true;
			this.checkConvertLoot.Checked = true;
			this.checkConvertLoot.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertLoot.Location = new System.Drawing.Point(6, 190);
			this.checkConvertLoot.Name = "checkConvertLoot";
			this.checkConvertLoot.Size = new System.Drawing.Size(47, 17);
			this.checkConvertLoot.TabIndex = 8;
			this.checkConvertLoot.Text = "Loot";
			this.checkConvertLoot.UseVisualStyleBackColor = true;
			// 
			// checkConvertLevels
			// 
			this.checkConvertLevels.AutoSize = true;
			this.checkConvertLevels.Checked = true;
			this.checkConvertLevels.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertLevels.Location = new System.Drawing.Point(6, 213);
			this.checkConvertLevels.Name = "checkConvertLevels";
			this.checkConvertLevels.Size = new System.Drawing.Size(57, 17);
			this.checkConvertLevels.TabIndex = 9;
			this.checkConvertLevels.Text = "Levels";
			this.checkConvertLevels.UseVisualStyleBackColor = true;
			// 
			// textLevelsXmlPath
			// 
			this.textLevelsXmlPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textLevelsXmlPath.Location = new System.Drawing.Point(106, 58);
			this.textLevelsXmlPath.Name = "textLevelsXmlPath";
			this.textLevelsXmlPath.Size = new System.Drawing.Size(249, 20);
			this.textLevelsXmlPath.TabIndex = 23;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(45, 61);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(55, 13);
			this.label11.TabIndex = 22;
			this.label11.Text = "levels.xml:";
			// 
			// buttonBrowseLevelsXml
			// 
			this.buttonBrowseLevelsXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowseLevelsXml.Location = new System.Drawing.Point(361, 56);
			this.buttonBrowseLevelsXml.Name = "buttonBrowseLevelsXml";
			this.buttonBrowseLevelsXml.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowseLevelsXml.TabIndex = 24;
			this.buttonBrowseLevelsXml.Text = "Browse...";
			this.buttonBrowseLevelsXml.UseVisualStyleBackColor = true;
			this.buttonBrowseLevelsXml.Click += new System.EventHandler(this.buttonBrowseLevelsXml_Click);
			// 
			// checkConvertSounds
			// 
			this.checkConvertSounds.AutoSize = true;
			this.checkConvertSounds.Checked = true;
			this.checkConvertSounds.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkConvertSounds.Location = new System.Drawing.Point(6, 236);
			this.checkConvertSounds.Name = "checkConvertSounds";
			this.checkConvertSounds.Size = new System.Drawing.Size(62, 17);
			this.checkConvertSounds.TabIndex = 10;
			this.checkConvertSounds.Text = "Sounds";
			this.checkConvertSounds.UseVisualStyleBackColor = true;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(474, 344);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormMain";
			this.Text = "Hammerwatch to A000FF converter";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackActorHealthScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackRangeScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackDamageScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackActorSpeedScale)).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button buttonConvertUnit;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button buttonConvert;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.CheckBox checkConvertActors;
		private System.Windows.Forms.CheckBox checkConvertProjectiles;
		private System.Windows.Forms.CheckBox checkConvertDoodads;
		private System.Windows.Forms.CheckBox checkConvertTilesets;
		private System.Windows.Forms.CheckBox checkConvertItems;
		private System.Windows.Forms.CheckBox checkConvertStrings;
		private System.Windows.Forms.CheckBox checkConvertSpeechStyles;
		private System.Windows.Forms.CheckBox checkConvertFonts;
		private System.Windows.Forms.CheckBox checkConvertLoot;
		private System.Windows.Forms.CheckBox checkConvertLevels;
		private System.Windows.Forms.Button buttonBrowseLevelsXml;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkConvertSounds;
		private System.Windows.Forms.TextBox textOutputPrefix;
		private System.Windows.Forms.TextBox textOutputPath;
		private System.Windows.Forms.TextBox textSourcePath;
		private System.Windows.Forms.TextBox textSourceFallbackPath;
		private System.Windows.Forms.TextBox textLevelsXmlPath;
		private System.Windows.Forms.TrackBar trackActorHealthScale;
		private System.Windows.Forms.TrackBar trackRangeScale;
		private System.Windows.Forms.TrackBar trackDamageScale;
		private System.Windows.Forms.TrackBar trackActorSpeedScale;
		private System.Windows.Forms.CheckBox checkModifyWallCollision;
		private System.Windows.Forms.TextBox textStringsKeyPrefix;
	}
}

