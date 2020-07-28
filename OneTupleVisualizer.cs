
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class OneTupleVisualizer : Visualizer {
    private double blockHeight;
    private int groupLines = 16;
    private ContextMenuStrip popup;
    private Color color = Color.Green;

    public OneTupleVisualizer(int windowSize, Cantordust cantordust, Form frame) :
        base(windowSize, cantordust) {
        blockHeight = blockWidth;
        cantordust.cdprint("about to execute createPopupMenu\n");
        createPopupMenu(frame);
    }

    public void createPopupMenu(Form frame){
        ContextMenuStrip popup = new ContextMenuStrip { Name="test1"};
        // add color options
        Dictionary<string, Color> colorButtons = new Dictionary<string, Color> {
            { "Green", Color.Green },
            { "Red", Color.Red },
            { "Blue", Color.Blue },
            { "Magenta", Color.Magenta },
            { "Cyan", Color.Cyan },
            { "Yellow", Color.Yellow },
            { "White", Color.White },
            { "Orange", Color.Orange },
            { "Pink", Color.Pink },
        };

        MenuStrip colors = new MenuStrip() { Name = "Colors" };
        foreach (var entry in colorButtons.ToList()) {
            string name = entry.Key;
            Color c = entry.Value;
            var colorMenuItem = new ToolStripMenuItem { Text = name };
            colorMenuItem.Click += delegate
            {
                color = c;
                Invalidate();
            };
            colors.Items.Add(colorMenuItem);
        }
        // add line options
        MenuStrip lines = new MenuStrip { Text = "Lines" };
        for (int i=0;i<9;i++) {
            int j = (int)Math.Pow(2,i);
            var colorMenuItem = new ToolStripMenuItem(j.ToString());
            colorMenuItem.Click += delegate
            {
                groupLines = j;
                Invalidate();
            };
            lines.Items.Add(colorMenuItem);
        }

        popup.Items.Add(colors);
        popup.Items.Add(lines);

        frame.MouseUp += (sender, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                popup.Show(frame, e.X, e.Y);
            }
        };
        this.ContextMenuStrip = popup;
        //frame.setVisible(false);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        dataMicroSlider.Minimum = dataMacroSlider.getValue();
        dataMicroSlider.Maximum = dataMacroSlider.getUpperValue();
        int low = dataMicroSlider.getValue();
        int high = dataMicroSlider.getUpperValue();
        gradientPlot(e.Graphics, low, high);
    }

    private void gradientPlot(Graphics g, int low, int high) {
        blockWidth = Width / (double)0xff;
        blockHeight = Height / (double)0xff;
        byte[] data = mainInterface.getData();
        byte[] byteArray = new byte[256*256];
        int freq;

        for(int i = 0; i < 256; i++){
            for(int j = 0; j < 256 * groupLines; j++){
                int dataIndex = low + i * 256 *groupLines + j;                
                if(dataIndex < high){
                    int p = i * 256 + (data[dataIndex] & 0xFF);
                    if(byteArray[p] == (j / 256 + 1) * (256 / groupLines) || byteArray[p] == 255){
                        continue;
                    }
                    byteArray[p] += (byte)(256 / groupLines);
                    if(byteArray[p] == 0){
                        byteArray[p] = (byte)255;
                    }
                }
            }
        }

        double x = 0;
        double y = 0;
        for(int i = 0; i < 256*256; i++) {
            // g.setColor(new Color(0, (int)byteArray[i] & 0xFF, 0));
            int red_rgb = color.R;
            int green_rgb = color.G;
            int blue_rgb = color.B;
            int diff = 256 - ((int)byteArray[i] & 0xFF);

            red_rgb = (red_rgb - diff) >= 0 ? (red_rgb-diff) : 0;
            green_rgb = (green_rgb - diff) >= 0 ? (green_rgb-diff) : 0;
            blue_rgb = (blue_rgb - diff) >= 0 ? (blue_rgb-diff) : 0;
                
            var brush = new SolidBrush(Color.FromArgb(red_rgb, green_rgb, blue_rgb));
            g.FillRectangle(brush, x*blockWidth, y*blockHeight, blockWidth, blockHeight);
            x++;
            if(x == 256){
                x = 0;
                y++;
            }
            brush.Dispose();
        }
    }

    public static int getWindowSize() {return 360;}
}