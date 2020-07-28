//import org.joml.Matrix4f;
//import org.lwjgl.BufferUtils;
//import org.lwjgl.opengl.*;
//import org.lwjgl.opengl.awt.AWTGLCanvas;
//import org.lwjgl.opengl.awt.GLData;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;
import javax.swing.*;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Image;
import java.awt.Insets;
import java.util.Arrays;
import java.util.HashMap;

public class MainInterface : UserControl {
    private byte[] data;
    private byte[] fullData;
    public BitMapSlider macroSlider;
    public BitMapSlider microSlider;
    public ScrollBar widthSlider;
    public ScrollBar offsetSlider;
    public ScrollBar dataSlider;
    public Button widthDownButton;
    public Button widthUpButton;
    public Button offsetDownButton;
    public Button offsetUpButton;
    public Button microUpButton;
    public Button hilbertMapButton;
    public Button themeButton;
    public Button twoTupleButton;
    public Button eightBitPerPixelBitMapButton;
    public Button byteCloudButton;
    public Button metricMapButton;
    public Button oneTupleButton;
    public ContextMenuStrip popup;

    private Cantordust cantordust;
    private Label dataRange = new Label();
    private Label macroValueHigh = new Label();
    private Label macroValueLow = new Label();
    private Label microValueHigh = new Label();
    private Label microValueLow = new Label();
    private Label widthValue = new Label();
    private Label offsetValue = new Label();
    private Label programName = new Label();

    private Panel currVis = new Panel();

    /* visualizers stored here so no duplicate visualizer instances are ever created.*/
    private Dictionary<visualizerMapKeys, Control> visualizerPanels;

    private enum visualizerMapKeys {
        BITMAP,
        BYTECLOUD,
        METRIC,
        TWOTUPLE,
        ONETUPLE
    }

    private Form frame;
    private string basePath;
    private int xOffset = 0;
    protected byte theme;
    protected bool dispMetricMap;

    public MainInterface(byte[] mdata, Cantordust cantordust, Form frame) {
        this.data = mdata;
        this.fullData = mdata;
        this.cantordust = cantordust;
        this.frame = frame;
        visualizerPanels = new HashMap<visualizerMapKeys, JPanel();

        this.dispMetricMap = false;
        this.basePath = cantordust.currentDirectory;

        //setBorder(BorderFactory.createEmptyBorder(6, 6, 6, 6));
        //setLayout(new GridBagLayout());
        //GridBagConstraints gbc = new GridBagConstraints();

        if(fullData.length > 26214400){
            // 0xfffff = 1048575, 25MB = 0x1900000 = 26214400 bytes
            this.data = Arrays.copyOfRange(fullData, 0, 1048575);
            int range = fullData.Length - 1048575;
            dataSlider = new VScrollBar { Minimum = 1, Maximum = range };
            dataSlider.Inverted = true;
            dataSlider.setValue(0);
            gbc.gridx = 0;
            gbc.gridy = 0;
            gbc.gridheight = 512;
            xOffset = 5;
            gbc.gridwidth = xOffset;
            gbc.fill = GridBagConstraints.BOTH;
            gbc.anchor = GridBagConstraints.CENTER;
            gbc.insets = new Insets(5, 5, 5, 5);
            add(dataSlider, gbc);
        }
        cantordust.cdprint("data: "+data.length+"\n");
        macroSlider = new BitMapSlider(1, this.data.length-1, this.data, this.cantordust);
        macroSlider.setValue(1);
        macroSlider.setUpperValue(this.data.length-1);
        gbc.gridx = xOffset + 0;
        gbc.gridy = 0;
        gbc.gridheight = 512;
        gbc.gridwidth = 10;
        gbc.fill = GridBagConstraints.BOTH;
        gbc.anchor = GridBagConstraints.CENTER;
        gbc.insets = new Insets(5, 5, 5, 5);
        add(macroSlider, gbc);
        
        microSlider = new BitMapSlider(0, this.data.length-1, this.data, this.cantordust);
        microSlider.setValue(macroSlider.getValue());
        microSlider.setUpperValue(macroSlider.getUpperValue());
        gbc.gridx = xOffset + 10;
        add(microSlider, gbc);

        Dimension incDim = new Dimension(18, 18);
        Insets zeroIn = new Insets(0, 0, 0, 0);

        microUpButton = new Button(">");
        microUpButton.addActionListener(new inc_micro());
        microUpButton.setPreferredSize(incDim);
        microUpButton.setMargin(zeroIn);
        microUpButton.setBorder(BorderFactory.createEmptyBorder());
        gbc.gridx = xOffset + 19;
        gbc.gridy = 512;
        gbc.gridheight = 1;
        gbc.gridwidth = 1;
        gbc.fill = GridBagConstraints.NONE;
        gbc.anchor = GridBagConstraints.EAST;
        add(microUpButton, gbc);

        widthDownButton = new Button("<");
        widthDownButton.addActionListener(new dec_width());
        widthDownButton.setPreferredSize(incDim);
        widthDownButton.setMargin(zeroIn);
        widthDownButton.setBorder(BorderFactory.createEmptyBorder());
        gbc.gridx = xOffset + 20;
        add(widthDownButton, gbc);

        Dimension slideDim = new Dimension(200, 15);

        widthSlider = new ScrollBar(1, 1024);
        widthSlider.setValue(512);
        widthSlider.setMaximum(1024);
        widthSlider.setOrientation(SwingConstants.HORIZONTAL);
        widthSlider.setPreferredSize(slideDim);
        gbc.gridy = 512;
        gbc.gridx = xOffset + 21;
        gbc.gridheight = 1;
        gbc.gridwidth = 1;
        gbc.fill = GridBagConstraints.NONE;
        add(widthSlider, gbc);

        widthUpButton = new Button(">");
        widthUpButton.addActionListener(new inc_width());
        widthUpButton.setPreferredSize(incDim);
        widthUpButton.setMargin(zeroIn);
        widthUpButton.setBorder(BorderFactory.createEmptyBorder());
        gbc.gridx = xOffset + 260;
        add(widthUpButton, gbc);

        offsetDownButton = new Button("<");
        offsetDownButton.addActionListener(new dec_offset());
        offsetDownButton.setPreferredSize(incDim);
        offsetDownButton.setMargin(zeroIn);
        offsetDownButton.setBorder(BorderFactory.createEmptyBorder());
        gbc.gridx = xOffset + 261;
        add(offsetDownButton, gbc);

        offsetSlider = new ScrollBar(1, 255);
        offsetSlider.setValue(0);
        offsetSlider.setMaximum(255);
        offsetSlider.setOrientation(SwingConstants.HORIZONTAL);
        offsetSlider.setPreferredSize(slideDim);
        gbc.gridx = xOffset + 270;
        add(offsetSlider, gbc);

        offsetUpButton = new Button(">");
        offsetUpButton.addActionListener(new inc_offset());
        offsetUpButton.setPreferredSize(incDim);
        offsetUpButton.setMargin(zeroIn);
        offsetUpButton.setBorder(BorderFactory.createEmptyBorder());
        gbc.gridx = xOffset + 512;
        add(offsetUpButton, gbc);
        
        // Default Current Visualization: MetricMap
        currVis = new MetricMap(MetricMap.getWindowSize(), cantordust, this, frame, true);
        currVis.setPreferredSize(new Dimension(512, 512));
        gbc.gridx = xOffset + 20;
        gbc.gridy = 0;
        gbc.gridheight = 512;
        gbc.gridwidth = 512;
        gbc.fill = GridBagConstraints.NONE;
        gbc.anchor = GridBagConstraints.CENTER;
        gbc.insets = new Insets(5, 5, 5, 5);
        add(currVis, gbc);

        // Setup buttons and button icons
        
        Image twoTupleIcon = ImageIO.read(new File(basePath + "resources/icons/icon_2_tuple.bmp")).getScaledInstance(41, 41, Image.SCALE_SMOOTH);
        twoTupleButton = new Button(new ImageIcon(twoTupleIcon));
        twoTupleButton.addActionListener(new open_two_tuple());
        twoTupleButton.setPreferredSize(new Dimension(50, 50));
        twoTupleButton.BackColor = (Color.darkGray);
        twoTupleButton.setToolTipText("Two Tuple");
        gbc.gridx = xOffset + 532;
        gbc.gridheight = 1;
        gbc.gridwidth = 1;
        add(twoTupleButton, gbc);

        Image bmpIcon = ImageIO.read(new File(basePath + "resources/icons/icon_bit_map.bmp")).getScaledInstance(41, 41, Image.SCALE_SMOOTH);
        eightBitPerPixelBitMapButton = new Button(new ImageIcon(bmpIcon));
        eightBitPerPixelBitMapButton.addActionListener(new open_8bpp_BitMap());
        eightBitPerPixelBitMapButton.setPreferredSize(new Dimension(50, 50));
        eightBitPerPixelBitMapButton.BackColor = (Color.darkGray);
        eightBitPerPixelBitMapButton.setToolTipText("Linear BitMap");
        gbc.gridy = 1;
        add(eightBitPerPixelBitMapButton, gbc);

        Image byteCloudIcon = ImageIO.read(new File(basePath + "resources/icons/icon_cloud.bmp")).getScaledInstance(41, 41, Image.SCALE_SMOOTH);
        byteCloudButton = new Button(new ImageIcon(byteCloudIcon));
        byteCloudButton.addActionListener(new open_byte_cloud());
        byteCloudButton.setPreferredSize(new Dimension(50, 50));
        byteCloudButton.BackColor = (Color.darkGray);
        byteCloudButton.setToolTipText("Byte Cloud");
        gbc.gridy = 2;
        add(byteCloudButton, gbc);
        
        Image metricMapIcon = ImageIO.read(new File(basePath + "resources/icons/icon_metricMap.png")).getScaledInstance(41, 41, Image.SCALE_SMOOTH);
        metricMapButton = new Button(new ImageIcon(metricMapIcon));
        metricMapButton.addActionListener(new open_metric_map());
        metricMapButton.setPreferredSize(new Dimension(50, 50));
        metricMapButton.BackColor = (Color.darkGray);
        metricMapButton.setToolTipText("Metric Map");
        gbc.gridy = 3;
        add(metricMapButton, gbc);

        Image oneTupleIcon = ImageIO.read(new File(basePath + "resources/icons/icon_1_tuple.bmp")).getScaledInstance(41, 41, Image.SCALE_SMOOTH);
        oneTupleButton = new Button(new ImageIcon(oneTupleIcon));
        oneTupleButton.addActionListener(new open_one_tuple());
        oneTupleButton.setPreferredSize(new Dimension(50, 50));
        oneTupleButton.BackColor = (Color.darkGray);
        oneTupleButton.setToolTipText("One Tuple");
        gbc.gridy = 4;
        add(oneTupleButton, gbc);
        
        themeButton = new Button("th");
        themeButton.addActionListener(new change_theme());
        gbc.gridy = 5;
        add(themeButton, gbc);

        long minGhidraAddress = Convert.ToInt64(cantordust.getCurrentProgram().getMinAddress().ToString(false), 16);
        long maxAddress = minGhidraAddress + macroSlider.getUpperValue(); 
        long minAddress = minGhidraAddress + macroSlider.getValue() - 1;

        macroValueHigh.Text = maxAddress.ToString("X");

        macroValueLow.Text = minAddress.ToString("X");

        maxAddress = minGhidraAddress + microSlider.getUpperValue();
        minAddress = minGhidraAddress + microSlider.getValue() - 1;
        
        programName.Text = cantordust.name;
        gbc.gridx = xOffset + 0;
        gbc.gridy = 513;
        //add(programName, gbc);

        microValueLow.setText(Long.toHexString(minAddress).toUpperCase());
        microValueLow.setHorizontalAlignment(SwingConstants.LEFT);
        gbc.gridx = xOffset + 5;
        gbc.gridwidth = 5;
        gbc.fill = GridBagConstraints.NONE;
        gbc.anchor = GridBagConstraints.EAST;
        add(microValueLow, gbc);

        dataRange.setText("-");
        gbc.gridx = xOffset + 10;
        gbc.gridwidth = 1;
        add(dataRange, gbc);

        microValueHigh.setText(Long.toHexString(maxAddress).toUpperCase());
        microValueHigh.setHorizontalAlignment(SwingConstants.LEFT);
        gbc.gridx = xOffset + 11;
        gbc.gridwidth = 5;
        add(microValueHigh, gbc);


        widthValue.setText(Integer.toHexString(widthSlider.getValue()).toUpperCase());
        widthValue.setHorizontalAlignment(SwingConstants.LEFT);

        offsetValue.setText(Integer.toHexString(offsetSlider.getValue()).toUpperCase());
        offsetValue.setHorizontalAlignment(SwingConstants.LEFT);
         
        // Add listener to update display.
        if(dataSlider != null){
            dataSlider.ValueChanged += (sender, e) =>
            {
                var slider = (ScrollBar)sender;
                data = Arrays.copyOfRange(fullData, dataSlider.getValue(), dataSlider.getValue() + 1048575);

                long minGhidraAddress1 = Long.parseLong(cantordust.getCurrentProgram().getMinAddress().toString(false), 16);
                // Update text for upper and lower value of microSlider
                long maxAddress1 = minGhidraAddress1 + dataSlider.getValue() + microSlider.getUpperValue();
                long minAddress1 = minGhidraAddress1 + dataSlider.getValue() + macroSlider.getValue() + microSlider.getValue() - 1;
                microValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
                microValueLow.setText(Long.toHexString(minAddress1).toUpperCase());
                if (slider.getValueIsAdjusting()) {
                    macroSlider.updateData(data);
                    microSlider.updateData(data);
                    macroSlider.ui.makeBitmapAsync(0, data.length);
                    microSlider.ui.makeBitmapAsync(macroSlider.getValue(), macroSlider.getUpperValue());
                }
            };
        }
        macroSlider.ValueChanged += (sender, e) => {
            BitMapSlider slider = (BitMapSlider)sender;
            long minGhidraAddress1 = Long.parseLong(cantordust.getCurrentProgram().getMinAddress().toString(false), 16);
            long maxAddress1 = minGhidraAddress1 + slider.getUpperValue();
            long minAddress1 = minGhidraAddress1 + slider.getValue() - 1;

            // Update text for upper and lower value
            macroValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
            macroValueLow.setText(Long.toHexString(minAddress1).toUpperCase());

            int max = microSlider.getMaximum();
            int min = microSlider.getMinimum();
            int high = microSlider.getUpperValue();
            int low = microSlider.getValue();
            double highRatio = (double)(high - min) / (double)(max - min);
            double lowRatio = (double)(low - min) / (double)(max - min);

            // Update the upper and lower value of microSlider
            microSlider.setMinimum(slider.getValue());
            microSlider.setMaximum(slider.getUpperValue());
            int nMax = microSlider.getMaximum();
            int nMin = microSlider.getMinimum();
            if (slider.getValue() - 1 > microSlider.getValue() - 1) {
                microSlider.setValue(slider.getValue());
            }
            if (slider.getUpperValue() < microSlider.getUpperValue()) {
                microSlider.setUpperValue(slider.getUpperValue());
            }
            microSlider.setUpperValue((int)(highRatio * (nMax - nMin)) + nMin);
            microSlider.setValue((int)(lowRatio * (nMax - nMin)) + nMin);

            // Update text for upper and lower value of microSlider
            if (dataSlider != null) {
                maxAddress1 = minGhidraAddress1 + dataSlider.getValue() + microSlider.getUpperValue();
                minAddress1 = minGhidraAddress1 + dataSlider.getValue() + microSlider.getValue() - 1;
                microValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
                microValueLow.setText(Long.toHexString(minAddress1).toUpperCase());
            } else {
                maxAddress1 = minGhidraAddress1 + microSlider.getUpperValue();
                minAddress1 = minGhidraAddress1 + microSlider.getValue() - 1 + slider.getValue();
                microValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
                microValueLow.setText(Long.toHexString(minAddress1).toUpperCase());
            }
            Invalidate();
        };
        microSlider.ValueChanged += (sender, e) =>
        {
                BitMapSlider slider = (BitMapSlider) e.getSource();
                long minGhidraAddress1 = Long.parseLong(cantordust.getCurrentProgram().getMinAddress().toString(false), 16);
                long maxAddress1 = minGhidraAddress1 + slider.getUpperValue();
                long minAddress1 = minGhidraAddress1 + slider.getValue();

                // Make sure the slider stays within its bounds
                if(macroSlider.getValue()-1 > slider.getValue()-1) {
                    slider.setMinimum(macroSlider.getValue());
                    slider.setValue(macroSlider.getValue());
                }
                if(macroSlider.getUpperValue() < slider.getUpperValue()) {
                    slider.setMaximum(macroSlider.getUpperValue());
                    slider.setUpperValue(macroSlider.getUpperValue());
                }

                // Update text for the slider
                if(dataSlider != null){
                    // cantordust.cdprint("max"+slider.getMaximum()+"\n");
                    // cantordust.cdprint("min"+slider.getMinimum()+"\n");
                    maxAddress1 = maxAddress1 + dataSlider.getValue();
                    minAddress1 = minAddress1 + dataSlider.getValue();
                    microValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
                    microValueLow.setText(Long.toHexString(minAddress1).toUpperCase());
                } else {
                    microValueHigh.setText(Long.toHexString(maxAddress1).toUpperCase());
                    microValueLow.setText(Long.toHexString(minAddress1).toUpperCase());
                }

                if(macroSlider.getValueIsAdjusting()) {
                    repaint();
                }
            };
        widthSlider.ValueChanged += (sender, e) =>
        {
            ScrollBar slider = (ScrollBar)e.getSource();
            widthValue.setText(Integer.toHexString(slider.getValue()).toUpperCase());
        };
        offsetSlider.ValueChanged += (sender, e) => {
            ScrollBar slider = (ScrollBar)e.getSource();
            offsetValue.setText(Integer.toHexString(slider.getValue()).toUpperCase());
        };

        darkTheme();
    }

    /*public changeDemo() {
        Button decLowerButton = new Button("decrease lower bound");
        Button incLowerButton = new Button("increase lower bound");
        Button decUpperButton = new Button("decrease upper bound");
        Button incUpperButton = new Button("increase upper bound");
    }*/
    
    public static int getWindowWidth() {
        return 900;
    }

    public static int getWindowHeight() {
        return 645;
    }

    public byte[] getData() {
        return this.data;
    }

    public Form getFrame() {
        return this.frame;
    }

    /**
     * Sets the current theme to dark
     */
    private void darkTheme() {
        this.theme = 1;
        setTheme(Color.Black, Color.White, Color.DarkGray);
    }

    /**
     * Sets the current theme to light
     */
    private void lightTheme() {
        this.theme = 0;
        Color c = UIManager.getColor("panelButtons.background");
        Color textColor = Color.Black;
        setTheme(c, textColor, c);
    }

    /**
     * Sets colors of various components
     */
    private void setTheme(Color c, Color textColor, Color buttonColor) {
        this.BackColor = c;

        this.widthSlider.BackColor = (c);
        this.offsetSlider.BackColor = (c);
        if(this.dataSlider != null){
            this.dataSlider.BackColor = (c);
        }

        this.macroSlider.backcolor = c;
        this.microSlider.BackColor = (c);

        this.macroValueHigh.ForeColor = (textColor);
        this.macroValueLow.ForeColor = (textColor);
        this.microValueHigh.ForeColor = (textColor);
        this.microValueLow.ForeColor = (textColor);
        this.widthValue.ForeColor = (textColor);
        this.offsetValue.ForeColor = (textColor);

        this.widthDownButton.BackColor = (c);
        this.widthDownButton.ForeColor = (textColor);

        this.widthUpButton.BackColor = (c);
        this.widthUpButton.ForeColor = (textColor);

        this.offsetDownButton.BackColor = (c);
        this.offsetDownButton.ForeColor = (textColor);

        this.offsetUpButton.BackColor = (c);
        this.offsetUpButton.ForeColor = (textColor);

        this.dataRange.ForeColor = (textColor);
        this.programName.ForeColor = (textColor);

        this.microUpButton.BackColor = (c);
        this.microUpButton.ForeColor = (textColor);

        this.themeButton.BackColor = (buttonColor);
        this.themeButton.ForeColor = (textColor);

        if(dispMetricMap) {
            currVis.BackColor = (c);
        }
    }

    private class open_one_tuple { // ActionListener {

        public void actionPerformed(ActionEvent e) {
            if (!(currVis is OneTupleVisualizer)) {
                if ((e.getModifiers() & ActionEvent.SHIFT_MASK) > 0) {
                    currVis.setVisible(false);
                    remove(currVis);
                    dispMetricMap = false;
                    if(!visualizerPanels.containsKey(visualizerMapKeys.ONETUPLE)) {
                        visualizerPanels.put(visualizerMapKeys.ONETUPLE, new OneTupleVisualizer(OneTupleVisualizer.getWindowSize(), cantordust, frame));
                    }
                    currVis = visualizerPanels.get(visualizerMapKeys.ONETUPLE);
                    //currVis = new OneTupleVisualizer(OneTupleVisualizer.getWindowSize(), cantordust, frame);
                    currVis.setPreferredSize(new Dimension(512, 512));
                    currVis.setVisible(true);
                    GridBagConstraints gbc = new GridBagConstraints();
                    gbc.gridx = xOffset + 20;
                    gbc.gridy = 0;
                    gbc.gridheight = 512;
                    gbc.gridwidth = 512;
                    gbc.fill = GridBagConstraints.NONE;
                    gbc.anchor = GridBagConstraints.CENTER;
                    gbc.insets = new Insets(5, 5, 5, 5);
                    add(currVis, gbc);
                    validate();
                } else {
                    //JOptionPane.showMessageDialog(null, "test", "InfoBox: " + "test", JOptionPane.INFORMATION_MESSAGE);
                    Form frame1 = new Form { Text = "1 Tuple Visualization" };
                    OneTupleVisualizer oneTupleVis = new OneTupleVisualizer(OneTupleVisualizer.getWindowSize(), cantordust, frame1);
                    frame1.Controls.Add(oneTupleVis);
                    frame1.Size = new Size(OneTupleVisualizer.getWindowSize(), OneTupleVisualizer.getWindowSize());
                    //frame.pack();
                    frame1.Show();
                    //frame1.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
                }
            }
        }
    }

    private class open_two_tuple { // : ActionListener {
        open_two_tuple() {

        }

        public void actionPerformed(ActionEvent e) {
            if (!(currVis is TwoTupleVisualizer)) {
                if ((e.getModifiers() & ActionEvent.SHIFT_MASK) > 0) {
                    currVis.Visible = false;
                    remove(currVis);
                    dispMetricMap = false;
                    if(!visualizerPanels.containsKey(visualizerMapKeys.TWOTUPLE)) {
                        visualizerPanels.put(visualizerMapKeys.TWOTUPLE, new TwoTupleVisualizer(TwoTupleVisualizer.getWindowSize(), cantordust, frame));
                    }
                    currVis = visualizerPanels.get(visualizerMapKeys.TWOTUPLE);
                    //currVis = new OneTupleVisualizer(OneTupleVisualizer.getWindowSize(), cantordust, frame);
                    currVis.setPreferredSize(new Dimension(512, 512));
                    currVis.setVisible(true);
                    GridBagConstraints gbc = new GridBagConstraints();
                    gbc.gridx = xOffset + 20;
                    gbc.gridy = 0;
                    gbc.gridheight = 512;
                    gbc.gridwidth = 512;
                    gbc.fill = GridBagConstraints.NONE;
                    gbc.anchor = GridBagConstraints.CENTER;
                    gbc.insets = new Insets(5, 5, 5, 5);
                    add(currVis, gbc);
                    validate();
                } else {
                    //JOptionPane.showMessageDialog(null, "test", "InfoBox: " + "test", JOptionPane.INFORMATION_MESSAGE);
                    Form frame1 = new Form { Text = "2 Tuple Visualization" };
                    TwoTupleVisualizer twoTupleVis = new TwoTupleVisualizer(TwoTupleVisualizer.getWindowSize(), cantordust, frame1);
                    frame1.getContentPane().add(twoTupleVis);
                    frame1.setSize(TwoTupleVisualizer.getWindowSize(), TwoTupleVisualizer.getWindowSize());
                    //frame.pack();
                    frame1.Visible = false;
                    //frame1.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
                }
            }
        }
    }

    private class open_8bpp_BitMap implements ActionListener {
        open_8bpp_BitMap() {

        }

        @Override
        public void actionPerformed(ActionEvent e) {
            if (!(currVis instanceof BitMapVisualizer)) {
                if ((e.getModifiers() & ActionEvent.SHIFT_MASK) > 0) {
                    currVis.setVisible(false);
                    remove(currVis);
                    dispMetricMap = false;
                    if(!visualizerPanels.containsKey(visualizerMapKeys.BITMAP)) {
                        cantordust.cdprint("map does not contain bitmap\n");
                        visualizerPanels.put(visualizerMapKeys.BITMAP, new BitMapVisualizer(BitMapVisualizer.getWindowSize(), cantordust, frame));
                    } else {cantordust.cdprint("map does contain bitmap\n");}
                    currVis = visualizerPanels.get(visualizerMapKeys.BITMAP);
                    currVis.setVisible(true);
                    currVis.setPreferredSize(new Dimension(512, 512));
                    GridBagConstraints gbc = new GridBagConstraints();
                    gbc.gridx = xOffset + 20;
                    gbc.gridy = 0;
                    gbc.gridheight = 512;
                    gbc.gridwidth = 512;
                    gbc.fill = GridBagConstraints.NONE;
                    gbc.anchor = GridBagConstraints.CENTER;
                    gbc.insets = new Insets(5, 5, 5, 5);
                    add(currVis, gbc);
                    validate();
                } else {
                    JFrame frame1 = new JFrame("Linear Bit Map");
                    BitMapVisualizer bitMapVis = new BitMapVisualizer(BitMapVisualizer.getWindowSize(), cantordust, frame1);
                    frame1.getContentPane().add(bitMapVis);
                    bitMapVis.setColorMapper(new EightBitPerPixelMapper(cantordust));
                    frame1.setSize(BitMapVisualizer.getWindowSize(), BitMapVisualizer.getWindowSize());
                    frame1.setVisible(true);
                    frame1.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
                }
            }
        }
    }

    private class open_byte_cloud implements ActionListener {
        open_byte_cloud() {
        	
        }

        @Override
        public void actionPerformed(ActionEvent e) {
            if (!(currVis instanceof ByteCloudVisualizer)) {
                if ((e.getModifiers() & ActionEvent.SHIFT_MASK) > 0) {
                    currVis.setVisible(false);
                    remove(currVis);
                    dispMetricMap = false;
                    if(!visualizerPanels.containsKey(visualizerMapKeys.BYTECLOUD)) {
                        visualizerPanels.put(visualizerMapKeys.BYTECLOUD, new ByteCloudVisualizer(ByteCloudVisualizer.getWindowSize(), cantordust));
                    }
                    currVis = visualizerPanels.get(visualizerMapKeys.BYTECLOUD);
                    currVis.setVisible(true);
                    currVis.setPreferredSize(new Dimension(512, 512));
                    GridBagConstraints gbc = new GridBagConstraints();
                    gbc.gridx = xOffset + 20;
                    gbc.gridy = 0;
                    gbc.gridheight = 512;
                    gbc.gridwidth = 512;
                    gbc.fill = GridBagConstraints.NONE;
                    gbc.anchor = GridBagConstraints.CENTER;
                    gbc.insets = new Insets(5, 5, 5, 5);
                    add(currVis, gbc);
                    validate();
                } else {
                    JFrame frame1 = new JFrame("Byte Cloud Visualization");
                    ByteCloudVisualizer byteCloudVis = new ByteCloudVisualizer(ByteCloudVisualizer.getWindowSize(), cantordust);
                    frame1.getContentPane().add(byteCloudVis);
                    frame1.setSize(ByteCloudVisualizer.getWindowSize(), ByteCloudVisualizer.getWindowSize());
                    frame1.setVisible(true);
                    frame1.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
                }
            }
        }
    }

    private class open_metric_map implements ActionListener {

        open_metric_map() {
        }

        @Override
        public void actionPerformed(ActionEvent e) {
            if (!(currVis instanceof MetricMap)) {
                if ((e.getModifiers() & ActionEvent.SHIFT_MASK) > 0) {
                    currVis.setVisible(false);
                    remove(currVis);
                    dispMetricMap = false;
                    if(!visualizerPanels.containsKey(visualizerMapKeys.METRIC)) {
                        visualizerPanels.put(visualizerMapKeys.METRIC, new MetricMap(MetricMap.getWindowSize(), cantordust, frame, true));
                    }
                    currVis = visualizerPanels.get(visualizerMapKeys.METRIC);
                    currVis.setPreferredSize(new Dimension(512, 512));
                    currVis.setVisible(true);
                    GridBagConstraints gbc = new GridBagConstraints();
                    gbc.gridx = xOffset + 20;
                    gbc.gridy = 0;
                    gbc.gridheight = 512;
                    gbc.gridwidth = 512;
                    gbc.fill = GridBagConstraints.NONE;
                    gbc.anchor = GridBagConstraints.CENTER;
                    gbc.insets = new Insets(5, 5, 5, 5);
                    add(currVis, gbc) ;
                    repaint();
                    validate();
                } else {
                    JFrame frame1 = new JFrame("Metric Map");
                    MetricMap metricMap = new MetricMap(MetricMap.getWindowSize(), cantordust, frame1, false);
                    frame1.getContentPane().add(metricMap);
                    frame1.setSize(metricMap.getWindowSize(), metricMap.getWindowSize()+30);
                    frame1.setVisible(true);
                    frame1.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
                }
            }
        }
    }

    private class dec_width implements ActionListener {
        dec_width() {
        	
        }
        @Override
        public void actionPerformed(ActionEvent e) {
            widthSlider.setValue(widthSlider.getValue() - 1);
        }
    }
    
    private class inc_width implements ActionListener {
        inc_width() {
        	
        }
    
        @Override
        public void actionPerformed(ActionEvent e) {
            widthSlider.setValue(widthSlider.getValue() + 1);
        }
    }
    
    private class dec_offset implements ActionListener {
        dec_offset() {
        	
        }
    
        @Override
        public void actionPerformed(ActionEvent e) {
            offsetSlider.setValue(offsetSlider.getValue() - 1);
        }
    }
    
    private class inc_offset implements ActionListener {
        inc_offset() {
        	
        }
    
        @Override
        public void actionPerformed(ActionEvent e) {
            offsetSlider.setValue(offsetSlider.getValue() + 1);
        }
    }
    
    private class inc_micro implements ActionListener {
        inc_micro() {
        	
        }
    
        @Override
        public void actionPerformed(ActionEvent e) {
            microSlider.setValue(microSlider.getValue() + 1);
        }
    }

    private class change_theme implements ActionListener {
        change_theme() {

        }

        @Override
        public void actionPerformed(ActionEvent e) {
            if(theme == 0) {
                // Swap to dark theme
                darkTheme();
            } else {
                // Swap to light theme
                lightTheme();
            }
        }
    }
}
