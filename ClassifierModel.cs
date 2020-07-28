
using System;
using System.IO;

public class ClassifierModel {
    private Cantordust cantordust;
    public readonly static string[] classes = {"arm4", "arm7", "ascii_english", "compressed", "java", "mips", "msil", "ones", "png",
                                "powerpc", "sparc_32", "utf_16_english", "x64", "x86", "x86_padding", "zeros", "embedded_image"};
    private NGramModel[] nGramModels = new NGramModel[classes.Length];
    private string basePath;
    private int grams;
    private int[] blockClassifications;
    public static int DEFAULT_GRAMS = 4;
    public static int BLOCK_SIZE = 12;



    public ClassifierModel(Cantordust cantordust, int grams) {
        basePath = cantordust.currentDirectory + "resources/templates/";
        this.grams = grams;
        this.cantordust = cantordust;
    }

    public void initialize(){
        for(int i=0; i < classes.Length; i++) {
            byte[] data = null;
            try {
                data = File.ReadAllBytes(basePath + classes[i] + ".template");
            } catch(IOException e) {
                e.printStackTrace();
            }
            cantordust.cdprint(String.Format("generated "+classes[i]+" ngram\n"));
            cantordust.cdprint("My stuff {\ndata: "+data.Length+"\n");
            cantordust.cdprint("grams: "+this.grams+"\n}\n");
            nGramModels[i] = new NGramModel(data, this.grams);
        }
        classifyData();
    }

    public int classify(byte[] data, int low, int high) {
        NGramModel model = new NGramModel(data, low, high - low, grams);
        ExponentialNotation p = model.EvaluateClassification(nGramModels[0]);
        int classification = 0;
        int i;
        for(i = 1; i < classes.Length; i++) {
            ExponentialNotation p1 = model.EvaluateClassification(nGramModels[i]);
            if(p1.greaterThan(p)) {
                p = p1;
                classification = i;
            }
        }
        return classification;
    }

    public void classifyData() {
        byte[] data = cantordust.getData();
        /*if(data.length % DEFAULT_BLOCK_SIZE > 0) {
            blockClassifications = new String[(data.length / DEFAULT_BLOCK_SIZE) + 1];
        } else {
            blockClassifications = new String[data.length / DEFAULT_BLOCK_SIZE];
        }*/
        blockClassifications = new int[(data.Length / BLOCK_SIZE)];
        for(int i=0; i < blockClassifications.Length; i++) {
            blockClassifications[i] = classify(data, i* BLOCK_SIZE, i* BLOCK_SIZE + BLOCK_SIZE);
            cantordust.cdprint(string.Format("block {0}-{1} : {2}\n", i, i+ BLOCK_SIZE, blockClassifications[i]));
        }

        /*for(int i = 0; i < 5000; i++) {
            cantordust.cdprint(String.format("class at index %d: %s\n", i, classAtIndex(i)));
        }*/
    }

    public int classAtIndex(int index) {
        cantordust.cdprint(string.Format("calling classAtIndex for index: {0} and getting block {1}\n", index, index / BLOCK_SIZE));
        try {
            return blockClassifications[index / BLOCK_SIZE];
        } catch(Exception e) {
            // Temporary. classifyData should account for last section of data. Will have to fix that.
            return 0;
        }
    }
}
