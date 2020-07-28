
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

public class TwoTupleVisualizer : Visualizer {
    private Bitmap img;
    private int divisions = 20;
    private List<Dictionary<TwoByteTuple, int>> cachedFreqMaps;
    HashSet<TwoByteTuple> existingTuples;
    private string colorMode = "g";
    private bool gradientMode = true;
    private int cycles = 0;

    public TwoTupleVisualizer(int windowSize, Cantordust cantordust, Form frame) :
        base(windowSize, cantordust)
    {
        this.img = null;
        initializeCaches();
        constructImageAsync();
        addChangeListeners();
        createPopupMenu(frame);
    }

    // Special constructor for initialization of plugin
    public TwoTupleVisualizer(int windowSize, Cantordust cantordust, MainInterface mainInterface, Form frame) :
        base(windowSize, cantordust, mainInterface) {
        this.img = null;
        initializeCaches();
        constructImageAsync();
        addChangeListeners();
        createPopupMenu(frame);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Rectangle window = ClientRectangle;

        if (img != null)
            e.Graphics.DrawImage(img, 0, 0, (int)window.Width, (int)window.Height);
    }

    public void constructImageAsync() {
        new Thread(() => {
            //initializeCaches();
            dataMicroSlider.setMinimum(dataMacroSlider.getValue());
            dataMicroSlider.setMaximum(dataMacroSlider.getUpperValue());
            int low = dataMicroSlider.getValue();
            int high = dataMicroSlider.getUpperValue();

        this.img = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            gradientPlot(Graphics.FromImage(img), low, high);
            Invalidate();
        }).start();
    }

    private Dictionary<TwoByteTuple, int> countedByteFrequencies(int low, int high) {
        // data needs fixed for large file sizes
        byte[] data = cantordust.mainInterface.getData();
        Dictionary<TwoByteTuple, int> tuples = new Dictionary<TwoByteTuple, int>();
        for(int tupleIdx=low; tupleIdx < high-1; tupleIdx++) {
            TwoByteTuple tuple = new TwoByteTuple(data[tupleIdx], data[tupleIdx+1]);
            if(tuples.TryGetValue(tuple, out int freq)) {
                existingTuples.Add(tuple);
                tuples[tuple] = freq + 1;
            } else {
                tuples.Add(tuple,  1);
            }
        }
        return tuples;
    }

    private void initializeCaches() {
        cachedFreqMaps = new List<Dictionary<TwoByteTuple, int>>();
        existingTuples = new HashSet<TwoByteTuple>();
        // data needs fixed for large file sizes
        byte[] data = cantordust.mainInterface.getData();
        int cachedSize = data.Length / divisions;
        for(int div = 0; div < divisions - 1; div++) {
            cachedFreqMaps.Add(countedByteFrequencies(div*cachedSize, (div+1)*cachedSize));
        }
        cachedFreqMaps.Add(countedByteFrequencies((divisions-1)*cachedSize, data.Length));
    }

    private void gradientPlot(Graphics g, int low, int high) {
        cycles += 1;
        // data needs fixed for large file sizes
        int cachedSize = cantordust.mainInterface.getData().Length / divisions;
        Dictionary<TwoByteTuple, int> totalFreqs = new Dictionary<TwoByteTuple, int>();
        Dictionary<TwoByteTuple, int> leftStraggler = null;
        Dictionary<TwoByteTuple, int> rightStraggler = null;
        int firstCacheBlockStart = nextBlock(low, cachedSize);
        int lastCacheBlockEnd = lastBlock(high, cachedSize);
        if(firstCacheBlockStart != low) {
            leftStraggler = countedByteFrequencies(low, firstCacheBlockStart-1);
            foreach (TwoByteTuple tuple in leftStraggler.Keys) {
                if(totalFreqs.ContainsKey(tuple)) {
                    totalFreqs.put(tuple, leftStraggler.get(tuple) + totalFreqs.get(tuple));
                } else {
                    totalFreqs.put(tuple, leftStraggler.get(tuple));
                }
            }
        }
        for(int currentBlock = firstCacheBlockStart / cachedSize; currentBlock <= lastCacheBlockEnd / cachedSize; currentBlock++) {
            mergeFreqCounts(cachedFreqMaps.get(currentBlock-1), totalFreqs);
        }

        int colorStep = 5;
        int min = 0;
        foreach (TwoByteTuple twoTuple in totalFreqs.Keys) {
            int freq = totalFreqs[twoTuple];
            //g.setColor(new Color(0, min + (freq*colorStep > 255 - min ? 255 - min : freq*colorStep), 0));
            var br = new SolidBrush(getColor(freq, colorMode));
            //int colorVal = min + (freq*colorStep > 255 - min ? 255 - min : freq*colorStep);
            //g.setColor(new Color(colorVal, colorVal, colorVal));
            int x = twoTuple.x & 0xff;
            int y = twoTuple.y & 0xff;
            // g.fill(new Rectangle2D.Double(x*blockWidth, y*blockWidth, blockWidth, blockWidth));
            g.FillRectangle(br, y, x, 1, 1);
            br.Dispose();
        }
        g.Dispose();
    }

    private void mergeFreqCounts(Dictionary<TwoByteTuple, int> sender, Dictionary<TwoByteTuple, int> reciever) {
        foreach (TwoByteTuple tuple in sender.Keys) {
            reciever[tuple] = (sender[tuple] + (reciever.ContainsKey(tuple) ? reciever[tuple] : 0));
        }
    }

    private int nextBlock(int x, int size) {
        if (x % size == 0) {
            return x;
        } else if (x < size) {
            return size;
        } else {
            return Math.floorDiv(x, size)*size + size;
        }
    }

    private int lastBlock(int x, int size) {
        return x - (x % size);
    }
        /*int colorStep = 10;
        int min = 60;
        for(TwoByteTuple twoTuple: tuples.keySet()) {
            freq = tuples.get(twoTuple);
            g.setColor(new Color(0, min + (freq*colorStep > 255 - min ? 255 - min : freq*colorStep), 0));
            //int colorVal = min + (freq*colorStep > 255 - min ? 255 - min : freq*colorStep);
            //g.setColor(new Color(colorVal, colorVal, colorVal));
            int x = twoTuple.x & 0xff;
            int y = twoTuple.y & 0xff;
            g.fill(new Rectangle2D.Double(x, y, 1, 1));
        }
    }*/

    public static int getWindowSize() {
        return 500;
    }

    private void addChangeListeners() {
        dataMacroSlider.ValueChanged += delegate
        {
            constructImageAsync();
        };
        dataMicroSlider.ValueChanged += delegate
        {
            constructImageAsync();
        };
    }

    private Color getColor(int freq, String rgbPosition) {
        int colorStep = 5;
        int min = 10;
        switch(rgbPosition) {
            case "r":
                if(gradientMode)
                    return new Color(min + (freq*colorStep > 255 - min ? 255 - min : freq*colorStep), 0, 0);
                else {
                    return Color.RED;
                }
            case "g":
                if(gradientMode) {
                    return new Color(0, min + (freq * colorStep > 255 - min ? 255 - min : freq * colorStep), 0);
                } else {
                    return Color.GREEN;
                }
            case "b":
                if(gradientMode) {
                    return new Color(0, 0, min + (freq * colorStep > 255 - min ? 255 - min : freq * colorStep));
                } else {
                    return Color.BLUE;
                }
            default:
                return null;
        }
    }

    public void createPopupMenu(JFrame frame){
        JPopupMenu popup = new JPopupMenu("colors");
        JMenuItem redItem = new JMenuItem("red");
        redItem.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                colorMode = "r";
                constructImageAsync();
            }
        });
        popup.add(redItem);

        JMenuItem greenItem = new JMenuItem("green");
        greenItem.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                colorMode = "g";
                constructImageAsync();
            }
        });
        popup.add(greenItem);

        JMenuItem blueItem = new JMenuItem("blue");
        blueItem.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                colorMode = "b";
                constructImageAsync();
            }
        });
        popup.add(blueItem);


        JMenuItem gradientToggle = new JMenuItem("toggle gradient");
        gradientToggle.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                gradientMode = !gradientMode;
                constructImageAsync();
            }
        });
        popup.add(gradientToggle);

        this.addMouseListener(new MouseAdapter() {
            public void mouseReleased(MouseEvent e) {
                if(e.getButton() == 3){
                    popup.show(frame, TwoTupleVisualizer.this.getX() + e.getX(), TwoTupleVisualizer.this.getY() + e.getY());
                }
            }
        });

        this.add(popup);
    }

}
