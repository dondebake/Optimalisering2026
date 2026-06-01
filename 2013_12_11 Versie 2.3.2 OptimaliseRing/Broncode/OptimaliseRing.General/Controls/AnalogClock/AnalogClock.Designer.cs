namespace OptimaliseRing.Controls.AnalogClock
{
  partial class AnalogClock
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
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.SuspendLayout();
      //
      // AnalogClock
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "AnalogClock";
      this.Resize += new System.EventHandler(this.AnalogClock_Resize);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.AnalogClock_Paint);
      this.ResumeLayout(false);

    }

  }
}
