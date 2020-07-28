using System.Drawing;
using System.Threading;
using System.Windows.Forms;

public class BitMapVisualizer : Visualizer {
    private ScrollBar dataWidthSlider;
    private ScrollBar dataOffsetSlider;
    private Button dataWidthDownButton;
    private Button dataWidthUpButton;
    private Button dataOffsetDownButton;
    private Button dataOffsetUpButton;
    private Button dataMicroUpButton;
    private int mode;

    private Image img;

    public BitMapVisualizer(int windowSize, Cantordust cantordust, Form frame) :
        base(windowSize, cantordust)
    { 
        dataWidthSlider = mainInterface.widthSlider;
        dataOffsetSlider = mainInterface.offsetSlider;
        dataWidthDownButton = mainInterface.widthDownButton;
        dataWidthUpButton = mainInterface.widthUpButton;
        dataOffsetDownButton = mainInterface.offsetDownButton;
        dataOffsetUpButton = mainInterface.offsetUpButton;
        dataMicroUpButton = mainInterface.microUpButton;
        mode = 0;
        this.img = new Bitmap(1,1);
        createPopupMenu(frame);

        dataMacroSlider.ValueChanged += delegate {
            constructImageAsync();
        };
        dataMicroSlider.ValueChanged += delegate {
            constructImageAsync();
        };

        dataWidthSlider.ValueChanged += delegate {
            constructImageAsync();
        };
        dataOffsetSlider.ValueChanged += delegate {
            constructImageAsync();
        };
        dataWidthDownButton.Click += delegate {
            constructImageAsync();
        };
        dataWidthUpButton.Click += delegate {
            constructImageAsync();
        };
        dataOffsetDownButton.Click += delegate {
            constructImageAsync();
        };
        dataOffsetUpButton.Click += delegate {
                constructImageAsync();
        };
        dataMicroUpButton.Click += delegate {
            constructImageAsync();
        };

        this.SizeChanged += delegate {
            constructImageAsync();
        };

        // Wait for the window to be loaded before building an image
        new Thread(() => {
            while(this.ClientRectangle.Width == 0) {
            	// Wait for the window to be loaded
            }

            constructImage();
        }).start();
    }

    public void createPopupMenu(Form frame) {
        ContextMenuStrip popup = new ContextMenuStrip() { Name = "test1" };
        ToolStripMenuItem bpp_8 = new ToolStripMenuItem("8bpp");
        bpp_8.Click += delegate {
            mode = 0;
            constructImageAsync();
        };
        popup.Items.Add(bpp_8);

        ToolStripMenuItem argb_32 = new ToolStripMenuItem("32bpp ARGB");
        argb_32.Click += delegate {
            mode = 1;
            constructImageAsync();
        };
        popup.Items.Add(argb_32);

        ToolStripMenuItem bpp_24 = new ToolStripMenuItem("24bpp RGB");
        bpp_24.Click += delegate {
            mode = 2;
            constructImageAsync();
        };
        popup.Items.Add(bpp_24);

        ToolStripMenuItem bpp_16 = new ToolStripMenuItem("16bpp ARGB1555");
        bpp_16.Click += delegate {
            mode = 3;
            constructImageAsync();
        };
        popup.Items.Add(bpp_16);

        ToolStripMenuItem entropy = new ToolStripMenuItem("Entropy");
        entropy.Click += delegate {
            mode = 4;
            constructImageAsync();
        };
        popup.Items.Add(entropy);

        this.MouseUp += (sender, e) =>
        {
            if (e.Button == MouseButtons.Right)
            {
                popup.Show(frame, this.Location.X + e.X, this.Location.Y + e.Y);
            }
        };
        this.ContextMenuStrip = popup;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (img != null)
            e.Graphics.DrawImage(img, 0, 0);
    }

    public void constructImageAsync() {
        new Thread(() => {
            constructImage();
        }).Start();
    }

    private void constructImage() {
        dataMicroSlider.Minimum = (dataMacroSlider.getValue());
        dataMicroSlider.Maximum = (dataMacroSlider.getUpperValue());
        int low = dataMicroSlider.getValue();
        int high = dataMicroSlider.getUpperValue();
        int width = dataWidthSlider.Value;
        int offset = dataOffsetSlider.Value;

        byte[] data = mainInterface.getData();
        byte[] data_offset = new byte[data.Length];
        int xMax = width;
        int y = 0;
        int x = 0;
        int i = 0;

        Rectangle window = ClientRectangle; //$ getVisibleRect

        for(i = 0; i < data.Length - offset; i++){
            data_offset[i] = data[i + offset];
        }
        for(i = data.Length-offset; i < data.Length; i++){
            data_offset[i] = 0;
        }

        Graphics g;
        Bitmap bimg;

        switch(mode) {
            //32bpp ARGB
            case 1:
                bimg = new Bitmap(width, (high - low) / xMax / 4 + 1); //$ BufferedImage.TYPE_INT_ARGB);
                g = Graphics.FromImage(bimg);
                for(i = low; i < high-4; i+=4) {
                    int pixel = ((data_offset[i+3] << 24) + (data_offset[i+2] << 16) + (data_offset[i+1] << 8) + data_offset[i]) & 0xFFFFFFFF;
                    g.setColor(new Color(pixel, true));
                    g.FillRectangle(new Rectangle2D.Double(x, y, 1, 1));
                    x++;
                    if(x == xMax) {
                        y++;
                        x = 0;
                    }
                }
                g.Dispose();
                break;

            //24bpp RGB
            case 2:
                bimg = new BufferedImage(width, (high-low)/xMax/3 + 1, BufferedImage.TYPE_INT_ARGB);
                g = bimg.createGraphics();
                for (i=low; i < high-3; i+=3){
                    int pixel = ((data_offset[i+2] << 16) + (data_offset[i+1] << 8) + data_offset[i]) & 0xFFFFFFFF;
                    g.setColor(new Color(pixel));
                    g.fill(new Rectangle2D.Double(x, y, 1, 1));
                    x++;
                    if(x == xMax) {
                        y++;
                        x = 0;
                    }
                }
                g.dispose();
                break;

            //16bpp ARGB1555 color is too saturated
            case 3:
                bimg = new BufferedImage(width, (high-low)/xMax/2 + 1, BufferedImage.TYPE_INT_ARGB);
                g = bimg.createGraphics();
                for (i = low; i < high-2; i+=2){
                    int alpha = (data_offset[i+1] & 0x80) >> 7;
                    float red = ((data_offset[i+1] & 0x7C) >> 2)/(0x1F);
                    float green = (((data_offset[i+1] & 0x03) << 3) + ((data[i] & 0xE0) >> 5))/(0x1F);
                    float blue  = (data_offset[i] & 0x1F)/(0x1F); 
                    g.setColor(new Color(red,green,blue,alpha));
                    g.fill(new Rectangle2D.Double(x, y, 1, 1));
                    x++;
                    if(x == xMax) {
                        y++;
                        x = 0;
                    }
                }
                g.dispose();
                break;

            // entropy
            case 4:
                bimg = new Bitmap(width, (high - low) / xMax + 1); //$ BufferedImage.TYPE_INT_ARGB);
                g = Graphics.FromImage(bimg);
                ColorEntropy entropy = new ColorEntropy(cantordust, data);
                for (i = low; i < high; i++){
                    Rgb rgb = entropy.getPoint(i);
                    g.setColor(new Color(rgb.r, rgb.g, rgb.b));
                    g.fill(new Rectangle2D.Double(x, y, 1, 1));
                    x++;
                    if(x == xMax) {
                        y++;
                        x = 0;
                    }
                }
                g.dispose();
                break;

            // 8bpp
            default:
                bimg = new BufferedImage(width, (high-low)/xMax + 1, BufferedImage.TYPE_INT_ARGB);
                g = bimg.createGraphics();
                for(i = low; i < high; i++){
                    int unsignedByte = data_offset[i] & 0xFF;
                    g.setColor(new Color(0,unsignedByte,0));
                    g.fill(new Rectangle2D.Double(x, y, 1, 1));
                    x++;
                    if(x == xMax) {
                        y++;
                        x = 0;
                    }
                }
                g.dispose();
            }
        // Scale the image
        this.img = bimg.getScaledInstance((int) window.getWidth(), (int) window.getHeight(), Image.SCALE_SMOOTH);
        repaint();
    }
    
    public static int getWindowSize() {
        return 800;
    }
}
