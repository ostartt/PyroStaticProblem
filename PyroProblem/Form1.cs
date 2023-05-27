using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static PyroProblem.PyroSolver;

namespace PyroProblem
{
  public partial class Form1 : Form
  {
    private double l;
    private int n;
    private int size;
    private static double h;

    private double lambda;
    private double pi;
    private double alpha;
    private double rho;
    private double c;
    private double e;
    private double g;
    private double sigmaL;
    private double dL;
    private double hL;

    private Matrix cBiForm;
    private Matrix eBiForm;
    private Matrix yBiForm;
    private Matrix gBiForm;
    private Matrix piBiForm;
    private Matrix kBiForm;
    private Vector rFunc;
    private Vector lFunc;
    private Vector muFunc;

    private List<double> xValues;
    private List<double> uValues;
    private List<double> pValues;
    private List<double> thetaValues;

    private static string seriesTitleU = "U";
    private static string seriesTitleP = "P";
    private static string seriesTitleTheta = "Theta";
    private void SetDefaultValues()
    {
      lambdaTextBox.Text = "1,16";
      piTextBox.Text = "0,00027";
      alphaTextBox.Text = "0,00002";
      rhoTextBox.Text = "7500";
      cTextBox.Text = "139000000000";
      eTextBox.Text = "-15,1";
      gTextBox.Text = "0,0000000646";
      sigmaLTextBox.Text = "0";
      dLTextBox.Text = "0";
      hLTextBox.Text = "100";
      nTextBox.Text = "8";
      lTextBox.Text = "0,01";
    }

    public Form1()
    {
      InitializeComponent();
      SetDefaultValues();
    }

    private void solveButton_Click(object sender, EventArgs eventArgs)
    {
      if (!IsInputCorrect())
      {
        MessageBox.Show("Input data is wrong", "Error message", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      else
      { 
        lambda = double.Parse(lambdaTextBox.Text);
        pi = double.Parse(piTextBox.Text);
        alpha = double.Parse(alphaTextBox.Text);
        rho = double.Parse(rhoTextBox.Text);
        c = double.Parse(cTextBox.Text);
        e = double.Parse(eTextBox.Text);
        g = double.Parse(gTextBox.Text);
        sigmaL = double.Parse(sigmaLTextBox.Text);
        dL = double.Parse(dLTextBox.Text);
        hL = double.Parse(hLTextBox.Text);
        n = int.Parse(nTextBox.Text);
        l = double.Parse(lTextBox.Text);
        h = l / n;
        
        Matrix derBlockCopy = 1 / h * new Matrix(DerBlock);
        
        size = 2 * n + 1;
        

      Matrix initialBiDerForm = GetInitialBiForm(derBlockCopy, n);
      Matrix initialBiDerFuncForm = GetInitialBiForm(DerFuncBlock, n);

      cBiForm = new Matrix(initialBiDerForm) * c;
      eBiForm = new Matrix(initialBiDerForm) * e;
      gBiForm = new Matrix(initialBiDerForm) * g;
      kBiForm = new Matrix(initialBiDerForm) * lambda;
      yBiForm = new Matrix(initialBiDerFuncForm) * alpha * c;
      piBiForm = new Matrix(initialBiDerFuncForm) * pi;

      lFunc = new Vector(size);
      rFunc = new Vector(size);
      muFunc = new Vector(size);

      lFunc[size - 1] = sigmaL;
      rFunc[size - 1] = dL;
      muFunc[size - 1] = hL;

      Matrix[,] finalMatrix = GetFinalMatrix(cBiForm, eBiForm, yBiForm, gBiForm, piBiForm, kBiForm, n);

      SetBoundaryValues(finalMatrix, Math.Pow(10, 20));

      Vector[] finalVector = GetFinalVector(lFunc, rFunc, muFunc, n);

      Vector[] solution = FiveDiagonalLowerUpperMethod(finalMatrix, finalVector);

      xValues = new List<double>();
      uValues = new List<double>();
      pValues = new List<double>();
      thetaValues = new List<double>();

      for (int i = 0; i < size; i++)
      {
        xValues.Add(i * h * 0.5);
        uValues.Add(solution[i][0]);
        pValues.Add(solution[i][1]);
        thetaValues.Add(solution[i][2]);
      }
      
      uChart.Series[seriesTitleU].Points.DataBindXY(xValues, uValues);
      pChart.Series[seriesTitleP].Points.DataBindXY(xValues, pValues);
      thetaChart.Series[seriesTitleTheta].Points.DataBindXY(xValues, thetaValues);
      }
    }

    private void resetButton_Click(object sender, EventArgs eventArgs)
    {
      lambdaTextBox.Text = string.Empty;
      piTextBox.Text = string.Empty;
      alphaTextBox.Text = string.Empty;
      rhoTextBox.Text = string.Empty;
      cTextBox.Text = string.Empty;
      eTextBox.Text = string.Empty;
      gTextBox.Text = string.Empty;
      sigmaLTextBox.Text = string.Empty;
      dLTextBox.Text = string.Empty;
      hLTextBox.Text = string.Empty;
      nTextBox.Text = string.Empty;
      lTextBox.Text = string.Empty;
      
      ClearChart();
    }

    private void setDefaultButton_Click(object sender, EventArgs eventArgs)
    {
      SetDefaultValues();
    }

    private bool IsInputCorrect()
    {
      return !Validator.IsTextEmpty(lambdaTextBox.Text, piTextBox.Text, alphaTextBox.Text, rhoTextBox.Text,
               cTextBox.Text, eTextBox.Text, gTextBox.Text, sigmaLTextBox.Text, dLTextBox.Text, hLTextBox.Text,
               nTextBox.Text, lTextBox.Text) 
             && Validator.IsDouble(lambdaTextBox.Text, piTextBox.Text, alphaTextBox.Text, rhoTextBox.Text,
               cTextBox.Text, eTextBox.Text, gTextBox.Text, sigmaLTextBox.Text, dLTextBox.Text, hLTextBox.Text) 
             && Validator.IsInt(nTextBox.Text);
    }

    private void getNormButton_Click(object sender, EventArgs eventArgs)
    {
      normDataGridView.ColumnCount = 4;
      normDataGridView.RowCount = 1;

      normDataGridView.Columns[0].HeaderText = "N";
      normDataGridView.Columns[1].HeaderText = "u norm";
      normDataGridView.Columns[2].HeaderText = "p norm";
      normDataGridView.Columns[3].HeaderText = "theta norm";
      
      normDataGridView.Rows[0].Cells[0].Value = n;
      normDataGridView.Rows[0].Cells[1].Value = GetNorm(uValues, n, h);
      normDataGridView.Rows[0].Cells[2].Value = GetNorm(pValues, n ,h);
      normDataGridView.Rows[0].Cells[3].Value = GetNorm(thetaValues, n, h);
    }

    private void ClearChart()
    {
      uChart.Series[seriesTitleU].Points.Clear();
      pChart.Series[seriesTitleP].Points.Clear();
      thetaChart.Series[seriesTitleTheta].Points.Clear();
    }
  }
}
