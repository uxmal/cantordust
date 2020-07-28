// metricMap

using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class MetricMap : Visualizer{
    protected byte[] data;
    protected static int size_hilbert = 512;
    protected int[][] pixelMap2D;
    protected int[] pixelMap1D;
    protected Dictionary<int, int> memLoc = new Dictionary<int, int>();
    protected Popup popupAddr;
    protected ContextMenuStrip popupMenu;
    protected Scurve map;
    protected Panel panel = new Panel();
    protected string type_plot = "square";
    protected ColorSource csource;
    private ScrollBar dataWidthSlider;
    private bool inMainInterface;
    private Label label;
    private bool isClassifier = false;

    public MetricMap(int windowSize, Cantordust cantordust, Form frame, bool isCurrentView) :
        base(windowSize, cantordust) {
        data = this.cantordust.mainInterface.getData();
        dataWidthSlider = mainInterface.widthSlider;
        createPopupMenu(frame);
        sliderConfig();
        mouseConfig(frame, isCurrentView);
        this.csource = new ColorEntropy(this.cantordust, getCurrentData());
        this.map = new Hilbert(this.cantordust, 2, (int)(Math.log(getWindowSize())/Math.log(2)));
        draw();
    }
    
    // Special constructor for initialization of plugin
    public MetricMap(int windowSize, Cantordust cantordust, MainInterface mainInterface, Form frame, bool isCurrentView) :
        base(windowSize, cantordust, mainInterface) {
        data = this.mainInterface.getData();
        dataWidthSlider = mainInterface.widthSlider;
        createPopupMenu(frame);
        sliderConfig();
        mouseConfig(frame, isCurrentView);
        this.csource = new ColorEntropy(this.cantordust, getCurrentData());
        this.map = new Hilbert(this.cantordust, 2, (int)(Math.Log(getWindowSize())/Math.Log(2)));
        Invalidate();
                Update();
    }

    public void sliderConfig(){
        this.dataMicroSlider.ValueChanged += delegate {
            Invalidate();
                Update();
        };
        this.dataMacroSlider.ValueChanged += delegate {
            Invalidate();
                Update();
        };
        if (dataRangeSlider != null) {
            this.dataRangeSlider.ValueChanged += delegate {
                data = cantordust.mainInterface.getData();
                Invalidate();
                Update();
            };
        }
        dataWidthSlider.ValueChanged += delegate
        {
            if (map.isType("linear"))
            {
                cantordust.cdprint("in linear, sliding\n");
                cantordust.cdprint("ch:" + dataWidthSlider.Value + "\n");
                int change = dataWidthSlider.Value;
                if (change < size_hilbert)
                {
                    cantordust.cdprint("less\n");
                    int inc = (size_hilbert - change) * 2;
                    change = size_hilbert + inc;
                    map.setWidth(change);
                    map.setHeight(size_hilbert);
                }
                else if (change > size_hilbert)
                {
                    cantordust.cdprint("more\n");
                    int inc = (change - size_hilbert) * 2;
                    change = size_hilbert + inc;
                    map.setHeight(change);
                    map.setWidth(size_hilbert);
                }
                else
                {
                    map.setWidth(size_hilbert);
                    map.setHeight(size_hilbert);
                }
                Invalidate();
                Update();
            }
        };
    }

    public void mouseConfig(Form frame, bool isCurrentView){
        this.MouseDown += (sender, e) => {
            // cantordust.cdprint("dragged\n");
            if (e.Button == MouseButtons.Left) {
                if (popupAddr != null) {
                    popupAddr.hide();
                }
                if (e.X < size_hilbert && e.Y < size_hilbert) {
                    if (e.X >= 0 && e.Y >= 0) {
                        //mousePressed(e);
                    }
                }
            }
        };
        //    void mousePressed(MouseEventArgs e) {
        //        MouseListener[] mA = getMouseListeners();
        //        if(mA.length >= 1) {
        //            mA[0].mousePressed(e);
        //        }
        //    }
        //});
        this.MouseDown += (sender, e) => {
                if ((e.Button == MouseButtons.Left) {
                    Control bv = this;
                    Form metricMap = frame;
                    int x_point=e.X;
                    int y_point=e.Y;
                    int xf = metricMap.Location.X+x_point;
                    int yf = metricMap.Location.Y+y_point;
                    if(isCurrentView){
                        var pt = bv.PointToScreen(new Point(x_point, y_point-26));
                        xf = pt.X;
                        yf = pt.Y;
                    }
                    TwoIntegerTuple p = new TwoIntegerTuple(x_point, y_point);
                    int currentLow = dataMicroSlider.getValue();
                    int loc = map.index(p);
                    if(isClassifier){
                        ClassifierModel classifier = cantordust.getClassifier();
                        //cantordust.cdprint(String.format("class at index %d : %s\n", loc, classifier.classes[classifier.classAtIndex(loc)]));
                    }
                    int memoryLocation = memLoc[loc]+currentLow;
                    if(dataRangeSlider != null){
                        memoryLocation = memoryLocation + cantordust.mainInterface.dataSlider.Value;
                    }
                    long minGhidraAddress = Long.parseLong(cantordust.getCurrentProgram().getMinAddress().toString(false), 16);
                    String currentAddress = Long.toHexString(minGhidraAddress+(long)memoryLocation).toUpperCase();
                    JLabel l;
                    if(isClassifier) {
                        ClassifierModel classifier = cantordust.getClassifier();
                        l = new JLabel(classifier.classes[classifier.classAtIndex(memoryLocation)]);
                    } else {
                        l = new JLabel(currentAddress);
                    }
                    PopupFactory pf = new PopupFactory(); 
                    JPanel p2 = new JPanel();
                    if(mainInterface.theme == 1) {
                        l.setForeground(Color.white);
                        p2.setBackground(Color.black);
                    }
                    p2.add(l);
                    popupAddr = pf.getPopup(metricMap, p2, xf, yf);
                    popupAddr.show();
                    
                    try{
                        // Set current location in Ghidra to this address
                        cantordust.gotoFileAddress(memoryLocation);
                    } catch(IllegalArgumentException exception){
                    }
                }
            }
            @Override
            public void mouseReleased(MouseEvent e) {
                if(popupAddr != null) {
                    popupAddr.hide();
                }
                if(e.getButton() == 3){
                    popupMenu.show(frame, MetricMap.this.getX() + e.X(), MetricMap.this.getY() + e.getY());
                }
            }
        });
    }
    
    public byte[] getCurrentData(){
        int lowerBound = dataMicroSlider.getValue();
        int upperBound = dataMicroSlider.getUpperValue();
        byte[] currentData = new byte[upperBound-lowerBound];
        // this.cantordust.cdprint(String.format("%d %d\n", lowerBound, upperBound));
        for(int i=lowerBound; i <= upperBound-2; i++) {
            currentData[i-lowerBound] = data[i];
        }
        return currentData;
    }

    public void createPopupMenu(JFrame frame){
        popupMenu = new JPopupMenu("Menu");
        ToolStripMenuItem pause = new ToolStripMenuItem("Pause");

        pause.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {    
                cantordust.cdprint("clicked pause\n");
            }
        });
        
        ToolStripMenuItem hilbert = new ToolStripMenuItem("Hilbert");
        hilbert.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if(!csource.type.equals("classifierPrediction")) {
                    isClassifier = false;
                }
                if(!map.isType("hilbert")) {
                    map = new Hilbert(cantordust, 2, (int)(Math.log(getWindowSize())/Math.log(2)));
                    draw();
                    cantordust.cdprint("clicked hilbert\n");
                } else { cantordust.cdprint("clicked hilbert\nAlready set\n"); }
            }
        });

        ToolStripMenuItem linear = new ToolStripMenuItem("Linear");
        linear.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!map.isType("linear")) {
                    cantordust.cdprint("clicked linear\n");
                    double x = Math.Pow(getWindowSize(),2);
                    map = new Linear(cantordust, 2, x);
                    draw();
                } else { cantordust.cdprint("clicked linear\nAlready set\n"); }
            }
        });

        ToolStripMenuItem zorder = new ToolStripMenuItem("Zorder");
        zorder.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!map.isType("zorder")) {
                    cantordust.cdprint("clicked zorder\n");
                    double x = Math.Pow(getWindowSize(),2);
                    map = new Zorder(cantordust, 2, x);
                    draw();
                } else { cantordust.cdprint("clicked zorder\nAlready set\n"); }
            }
        });

        ToolStripMenuItem hcurve = new ToolStripMenuItem("HCurve");
        hcurve.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!map.isType("hcurve")) {
                    cantordust.cdprint("clicked hcurve\n");
                    double x = Math.Pow(getWindowSize(),2);
                    map = new HCurve(cantordust, 2, x);
                    draw();
                } else { cantordust.cdprint("clicked hcurve\nAlready set\n"); }
            }
        });

        ToolStripMenuItem _8bpp = new ToolStripMenuItem("8bpp");
        _8bpp.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("8bpp")) {
                    csource = new Color8bpp(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked 8bpp\n");
                } else { cantordust.cdprint("clicked 8bpp\nAlready set\n"); }
            }
        });

        ToolStripMenuItem _16bpp = new ToolStripMenuItem("16bpp ARGB1555");
        _16bpp.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("16bpp ARGB1555")) {
                    csource = new Color16bpp_ARGB1555(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked 16bpp\n");
                } else { cantordust.cdprint("clicked 16bpp\nAlready set\n"); }
            }
        });

        ToolStripMenuItem _24bpp = new ToolStripMenuItem("24bpp Rgb");
        _24bpp.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("24bpp")) {
                    csource = new Color24bpp(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked 24bpp\n");
                } else { cantordust.cdprint("clicked 24bpp\nAlready set\n"); }
            }
        });

        ToolStripMenuItem _32bpp = new ToolStripMenuItem("32bpp Rgb");
        _32bpp.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("32bpp")) {
                    csource = new Color32bpp(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked 32bpp\n");
                } else { cantordust.cdprint("clicked 32bpp\nAlready set\n"); }
            }
        });
        
        ToolStripMenuItem _64bpp = new ToolStripMenuItem("64bpp Rgb");
        _64bpp.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("64bpp")) {
                    csource = new Color64bpp(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked 64bpp\n");
                } else { cantordust.cdprint("clicked 64bpp\nAlready set\n"); }
            }
        });

        ToolStripMenuItem entropy = new ToolStripMenuItem("Entropy");
        entropy.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("entropy")) {
                    csource = new ColorEntropy(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked entropy\n");
                } else { cantordust.cdprint("clicked entropy\nAlready set\n"); }
            }
        });

        ToolStripMenuItem byteClass = new ToolStripMenuItem("Byte Class");
        byteClass.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("class")) {
                    csource = new ColorClass(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked byte class\n");
                } else { cantordust.cdprint("clicked byte class\nAlready set\n"); }
            }
        });

        ToolStripMenuItem gradient = new ToolStripMenuItem("Gradient");
        gradient.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("gradient")) {
                    csource = new ColorGradient(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked gradient\n");
                } else { cantordust.cdprint("clicked gradient\nAlready set\n"); }
            }
        });

        ToolStripMenuItem spectrum = new ToolStripMenuItem("Spectrum");
        spectrum.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {   
                if(!csource.isType("spectrum")) {
                    csource = new ColorSpectrum(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked spectrum\n");
                } else { cantordust.cdprint("clicked spectrum\nAlready set\n"); }
            }
        });

        ToolStripMenuItem prediction = new ToolStripMenuItem("Classifier prediction");
        prediction.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                if(!csource.isType("classifierPrediction")) {
                    isClassifier = true;
                    csource = new ColorClassifierPrediction(cantordust, getCurrentData());
                    draw();
                    cantordust.cdprint("clicked classifier prediction\n");
                } else { cantordust.cdprint("clicked classifier prediction\nAlready set\n"); }
            }
        });
        
        /*ToolStripMenuItem stopClassifier = new ToolStripMenuItem("Stop Classifier");
        stopClassifier.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                isClassifier = false;
                popupMenu.remove(stopClassifier);
                cantordust.cdprint("Classifier Stopped\n");
            }
        });*/

        ToolStripMenuItem close = new ToolStripMenuItem("Close");
        close.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {    
                cantordust.cdprint("clicked close\n");
            }
        });

        ToolStripMenuItem classGen = new ToolStripMenuItem("Generate Classifier");
        classGen.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {    
                // draw();
                cantordust.cdprint("clicked Generate Classifier\n");
                //isClassifier = true;
                cantordust.initiateClassifier();
                popupMenu.remove(close);
                //popupMenu.add(stopClassifier);
                popupMenu.add(close);
                cantordust.cdprint("generated classifier\n");
            }
        });

        JMenu locality = new JMenu("Locality");
        locality.add(hilbert);
        locality.add(linear);
        locality.add(zorder);
        locality.add(hcurve);
        
        JMenu byteColor = new JMenu("Byte");
        byteColor.add(_8bpp);
        byteColor.add(_16bpp);
        byteColor.add(_24bpp);
        byteColor.add(_32bpp);
        byteColor.add(_64bpp);
        
        JMenu shading = new JMenu("Shading");
        shading.add(byteColor);
        shading.add(byteClass);
        shading.add(entropy);
        shading.add(gradient);
        shading.add(spectrum);
        shading.add(prediction);
        
        popupMenu.add(pause);
        popupMenu.add(locality);
        popupMenu.add(shading);
        popupMenu.add(classGen);
        popupMenu.add(close);
        this.add(popupMenu);
    }

    private void draw() {
        if(!csource.type.equals("classifierPrediction")) {
            isClassifier = false;
        }
        new Thread(() -> {
            // prog = progress.Progress(None);
            this.csource.setData(getCurrentData());
            if(csource.isType("spectrum")){
                this.csource = new ColorSpectrum(cantordust, getCurrentData());
            }
            if(type_plot.equals("unrolled")) {
                this.cantordust.cdprint("building unrolled "+this.map.type+" curve\n");
                drawMap_unrolled(this.map.type, size_hilbert, csource/*, dst, prog*/);
            }
            else if(type_plot.equals("square")){
                this.cantordust.cdprint("Building square "+this.map.type+" curve\n");
                drawMap_square(csource/*, dst, prog*/);
            }
       }).start();
    }

    public void drawMap_square(ColorSource csource/*, String name, prog */) {
        // prog.set_target(Math.Pow(size, 2))
        // if(this.map.isType("hilbert")){
        //     cantordust.cdprint("")
        //     this.map = new Hilbert(this.cantordust, 2, (int)(Math.log(getWindowSize())/Math.log(2)));
        // } else if (this.map.isType("zigzag")){
        //     this.map = new ZigZag(this.cantordust, 2, (double)getWindowSize());
        // }
        create2DPixelMap(this.map.dimensions());
        memLoc = new HashMap<Integer, Integer>();
        float step = (float)csource.getLength()/(float)(this.map.getLength());
        for(int i=0;i<this.map.getLength();i++){
            TwoIntegerTuple p = (TwoIntegerTuple)this.map.point(i);
            Rgb c = csource.point((int)(i*step));
            add2DPixel(p, c);
            addMemLoc(i, (int)(i*step));
        }
        convertPixelMapTo1D();
        //c.save(name);
        plotMap(this.map.dimensions());
    }

    public void drawMap_unrolled(String map_type, int size, ColorSource csource/*, String name, prog */) {
        cantordust.cdprint("draw unrolled map in-progress");
    }
    
    public void create2DPixelMap(TwoIntegerTuple dimensions){
        int width = dimensions.get(0);
        int height = dimensions.get(1);
        this.pixelMap2D = new int[height][width*3];
    }
    
    public void add2DPixel(TwoIntegerTuple point, Rgb color){
        /*
        adds 2D pixel to pixelMap2D
        */
        int x = point.get(0)*3;
        int y = point.get(1);
        this.pixelMap2D[y][x] = color.r;
        this.pixelMap2D[y][x+1] = color.g;
        this.pixelMap2D[y][x+2] = color.b;
    }

    public void addMemLoc(int idx, int rloc) {
        /*
        adds Memory relative memory location to XY coordinate
        */
        this.memLoc.put(idx, rloc);
    }
    
    public void flip2DPixlMap(TwoIntegerTuple dimensions){
        /*
        flips every other row in pixels to fit raster image
        */
        int width = dimensions.get(0);
        int height = dimensions.get(1);
        for(int row = 0; row < height; row++){
            if(!(row%2==0)){
                int[] temp = this.pixelMap2D[row];
                int j = width*3-4;
                for(int i = 0; i < width*3; i+=3){
                    this.pixelMap2D[row][i] = temp[j];
                    this.pixelMap2D[row][i+1] = temp[j+1];
                    this.pixelMap2D[row][i+2] = temp[j+2];
                    j-=3;
                }
            }
        }
    }
    
    public void convertPixelMapTo1D(){
        /*
        convert 2D Pixel Map to 1D Pixel Map (pixels)
        */
        this.pixelMap1D = new int[this.pixelMap2D.length*this.pixelMap2D[0].length];
        int x = 0;
        for(int i=0; i<this.pixelMap2D.length; i++) {
            for(int j=0; j<pixelMap2D[i].length; j++) {
                this.pixelMap1D[x] = this.pixelMap2D[i][j];
                x++;
            }
        }
    }
    
    public void plotMap(TwoIntegerTuple dimensions){
        int width = dimensions.get(0);
        int height = dimensions.get(1);
        // int imageSize = width * height * 3;
        // JPanel panel = new JPanel();
        // getContentPane().removeAll();
        // getContentPane().add(panel);
        // panel.add( createImageLabel(this.pixelMap1D, width, height) );
        // panel.revalidate();
        // panel.repaint();
        removeAll();
        add( createImageLabel(this.pixelMap1D, width, height) );
        revalidate();
        repaint();
    }
    
    private Label createImageLabel(int[] pixels, int width, int height)
    {
        // int change = size_hilbert - (int)((width - size_hilbert)/2);
        // cantordust.cdprint("ch: "+change+"\n");
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
        // cantordust.cdprint("w: "+width+"\nh: "+height+"\n");
        WritableRaster raster = image.getRaster();
        raster.setPixels(0, 0, width, height, pixels);
        label = new JLabel( new ImageIcon(image) );
        return label;
    }

    public static int getWindowSize() {
        return size_hilbert;
    }
}