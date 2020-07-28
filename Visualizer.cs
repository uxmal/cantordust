using System.Drawing;
using System.Windows.Forms;

public abstract class Visualizer : UserControl {
    protected MainInterface mainInterface;
    protected Cantordust cantordust;
    protected float blockWidth;
    protected RangeSlider dataMacroSlider;
    protected RangeSlider dataMicroSlider;
    protected ScrollBar dataRangeSlider;
    protected ColorMapper colorMapper;

    public Visualizer(int windowSize, Cantordust cantordust) {
        this.mainInterface = cantordust.mainInterface;
        this.cantordust = cantordust;
        dataMacroSlider = mainInterface.macroSlider;
        dataMicroSlider = mainInterface.microSlider;
        dataRangeSlider = mainInterface.dataSlider;
        BackColor = Color.Black;
        dataMacroSlider.ValueChanged += delegate {
                Invalidate();
        };

        dataMicroSlider.ValueChanged += delegate {
            Invalidate();
        };
        if (dataRangeSlider != null) {
            dataRangeSlider.ValueChanged += delegate
            {
                Invalidate();
            };
        }
    }

    // Special constructor for initialization of plugin
    public Visualizer(int windowSize, Cantordust cantordust, MainInterface mainInterface) {
        this.mainInterface = mainInterface;
        this.cantordust = cantordust;
        dataMacroSlider = mainInterface.macroSlider;
        dataMicroSlider = mainInterface.microSlider;
        dataRangeSlider = mainInterface.dataSlider;
        BackColor = Color.Black;
        dataMacroSlider.ValueChanged += delegate
        {
            Invalidate();
        };
        dataMicroSlider.ValueChanged += delegate
        {
            Invalidate();
        };
        if (dataRangeSlider != null)
        {
            dataRangeSlider.ValueChanged += delegate
                {
                    Invalidate();
                };
        }
    }

    public void setColorMapper(ColorMapper mapper) {
        colorMapper = mapper;
    }
}