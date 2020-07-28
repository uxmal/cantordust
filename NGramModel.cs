using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NGramModel {
    ExponentialNotation MINIMUM_FREQUENCY_MULTIPLIER =  new ExponentialNotation(1, -20);

    private int n;
    private int modelEntries;
    private Dictionary<VectorN, double> model = new Dictionary<VectorN, double>();

    public NGramModel(byte[] data, int startIndex, int length, int n) {
        GenerateModel(data, startIndex, length, n);
    }

    public NGramModel(byte[] data, int n) {
        GenerateModel(data, 0, data.Length, n );
    }

    private void recordInstance(VectorN v) {
        if(model.ContainsKey(v)) {
            model[v] = model[v] + 1;
        } else {
            model.Add(v, 1.0);
        }
    }

    public void GenerateModel(byte[] data, int startIndex, int length, int n) {
        this.n = n;

        // count occurrences of each existing n-gram
        for(int i = startIndex; i < startIndex + length; i++) {
            if(i + n < startIndex + length) {
                VectorN v = new VectorN(n);
                for(int k = 0; k < n; k++) {
                    v.setAt(k, data[i+k]);
                }
                if(model.ContainsKey(v)) {
                    model[v] = model[v] + 1;
                } else {
                    model.Add(v, 1.0);
                }
            }
        }

        // calculate probabilities
        modelEntries = data.Length - n + 1;
        foreach (VectorN entry in model.Keys.ToList()) {
            model[entry] = model[entry] / (double)modelEntries;
        }
    }

    public ExponentialNotation EvaluateClassification(NGramModel templateModel) {
        // ensure dimensionality is consistent
        if(n != templateModel.n) {throw new ArgumentException("inconsistent dimensions");}

        ExponentialNotation p = new ExponentialNotation(1);

        foreach (VectorN v in model.Keys.ToList()) {
            int k = (int)(model[v] * modelEntries + 0.5);
            if(templateModel.model.ContainsKey(v)) {
                double pClass = templateModel.model[v];
                p = p.multiply(MathUtils.fastPow(new ExponentialNotation(pClass), k));

            } else {
                p = p.multiply((new ExponentialNotation(1).divide(new ExponentialNotation(modelEntries))).multiply(MINIMUM_FREQUENCY_MULTIPLIER));
            }
        }
        return p;
    }

    // mostly for debug purposes
    public override string ToString() {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} entries: ", modelEntries);
        builder.AppendLine();
        foreach (VectorN key in model.Keys) {
            builder.AppendFormat("\t{0} : {1} ({2})\n", key.ToString(), (int)(model[key]*modelEntries), model[key]);
        }
        return builder.ToString();
    }
}