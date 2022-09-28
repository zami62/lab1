using System;
using System.Collections.Generic;
using System.Linq;

public static class Characters
{   
    public static char[] AvailableDigits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    public static char[] AvailableOperators = new char[] { '+', '-', '*', '/' };
    public static char[] AvailableBrackets = new char[] { '(', ')' };
}

public class CalculationBlock
{
    protected List<CalculationBlock> childBlocks = new List<CalculationBlock>();

    protected string inputString = "";

    protected int leftBracketsCount = 0;
    protected int rightBracketsCount = 0;

    protected string sValue = "";
    protected float fValue = 0;

    public CalculationBlock() { }

    public CalculationBlock(string sValue)
    {
        this.sValue = sValue;
    }


    protected bool IsADigit(char ch)
    {
        if (Characters.AvailableDigits.Contains(ch)) { return true; }
        else { return false; }
    }

    protected bool IsAnOperator(char ch)
    {
        if (Characters.AvailableOperators.Contains(ch)) { return true; }
        else { return false; }
    }

    protected bool IsABracket(char ch)
    {
        if (Characters.AvailableDigits.Contains(ch)) { return true; }
        else { return false; }
    }

    protected bool IsAPeriod(char ch)
    {
        if (ch == '.') { return true; }
        else { return false; }
    }


    private void DivideIntoBlocks()
    {
        if ((inputString != ""))
        {
            int i = 1;
            string value = "";

            CalculationBlock bracketsBlock = new CalculationBlock();

            while (i < inputString.Length - 1)
            {
                if ((inputString[i] == '(')) { leftBracketsCount++; }             

                if (leftBracketsCount != rightBracketsCount)
                {
                    bracketsBlock.inputString += inputString[i];
                }
                else if (leftBracketsCount == rightBracketsCount)
                {
                    if ((IsADigit(inputString[i])) || (inputString[i] == '.'))
                    {
                        value += inputString[i];
                    }
                    else if (IsAnOperator(inputString[i]))
                    {
                        if (value != "")
                        {
                            childBlocks.Add(new CalculationBlock(value));
                            //Console.WriteLine("(Block) Added a number before an operator" + value);
                            value = "";
                        }

                        childBlocks.Add(new CalculationBlock(inputString[i].ToString()));
                        //Console.WriteLine("(Block) Added an operator " + inputString[i].ToString());
                    }

                    if (IsABracket(inputString[i + 1]))
                    {
                        if (value != "")
                        {
                            childBlocks.Add(new CalculationBlock(value));
                            //Console.WriteLine("(Block) Added a number before a bracket " + value);
                            value = "";
                        }
                    }
                }

                if ((inputString[i] == ')')) { rightBracketsCount++; }

                if (leftBracketsCount == rightBracketsCount)
                {
                    if (bracketsBlock.inputString != "")
                    {
                        childBlocks.Add(bracketsBlock);
                        bracketsBlock = new CalculationBlock();
                    }
                }

                i++;
            }
        }
    }

    private void CastChildBlocks()
    {   
        if ((childBlocks.Count != 0) && (childBlocks[0].sValue == "-"))
        {
            childBlocks[0].sValue += childBlocks[1].sValue;
            inputString = "";
            childBlocks.RemoveAt(1);

            if (childBlocks.Count == 1) 
            {
                sValue = childBlocks[0].sValue;
                childBlocks.Clear();
            }
        }

        if ((childBlocks.Count == 0) && (IsADigit(sValue.Last())))
        {
            fValue = float.Parse(sValue);
        }
    }

    private void CalcChildBlocks()
    {
        if (childBlocks.Count > 2)
        {
            int i = 1;

            while (i < childBlocks.Count - 1)
            {
                if (childBlocks[i].sValue == "*")
                {
                    childBlocks[i - 1].fValue = childBlocks[i - 1].fValue * childBlocks[i + 1].fValue;
                    childBlocks.RemoveRange(i, 2);
                }
                else if (childBlocks[i].sValue == "/")
                {
                    childBlocks[i - 1].fValue = childBlocks[i - 1].fValue / childBlocks[i + 1].fValue;
                    childBlocks.RemoveRange(i, 2);
                }
                else
                {
                    i++;
                }
            }

            i = 1;

            while (i < childBlocks.Count - 1)
            {
                if (childBlocks[i].sValue == "+")
                {
                    childBlocks[i - 1].fValue = childBlocks[i - 1].fValue + childBlocks[i + 1].fValue;
                    childBlocks.RemoveRange(i, 2);

                }
                else if (childBlocks[i].sValue == "-")
                {
                    childBlocks[i - 1].fValue = childBlocks[i - 1].fValue - childBlocks[i + 1].fValue;
                    childBlocks.RemoveRange(i, 2);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    protected void CalcBlock()
    {
        //Console.WriteLine(inputString);

        DivideIntoBlocks();

        CastChildBlocks();

        foreach (CalculationBlock child in childBlocks)
        {
            child.CalcBlock();
        }

        CalcChildBlocks();

        if (childBlocks.Count != 0) { fValue = childBlocks[0].fValue; }
    }
}

public class MainCalculationBlock : CalculationBlock
{
    private string response = "";

    public MainCalculationBlock() { }

    public MainCalculationBlock(string inputString)
    {
        this.inputString = inputString;
    }

    private bool CheckInputString()
    {
        bool isInputValid = true;

        bool ErrInvChar = false;
        bool ErrInvFirstChar = false;
        bool ErrDivByZero = false;
        bool ErrMultOperators = false;
        bool ErrMultPeriods = false;
        bool ErrMultZeros = false;
        bool ErrNumStartsWithZero = false;

        int lBracketsCount = 0;
        int rBracketsCount = 0;



        if (!((inputString[0] == '-') || (inputString[0] == '(') || IsADigit(inputString[0])))
        {
            ErrInvFirstChar = true;
        }

        for (int i = 0; i < inputString.Length; i++)
        {
            if (inputString[i] == '(') { lBracketsCount++; }
            else if (inputString[i] == ')') { rBracketsCount++; }

            if (!(IsADigit(inputString[i]) 
                || IsAnOperator(inputString[i]) 
                || IsABracket(inputString[i]) 
                || (inputString[i] == '.')))
            {
                ErrInvChar = true;        
            }

            if (i < inputString.Length - 1)
            {
                if ((IsAnOperator(inputString[i])) && (IsAnOperator(inputString[i + 1])))
                {
                    ErrMultOperators = true;
                }
                if ((inputString[i] == '.') && (inputString[i + 1] == '.'))
                {
                    ErrMultPeriods = true;
                }
                if ((inputString[i] == '0') && (inputString[i + 1] == '0'))
                {
                    ErrMultZeros = true;
                }
                if ((inputString[i] == '0') && (IsADigit(inputString[i]) && (inputString[i] != '0')))
                {
                    ErrNumStartsWithZero = true;
                }
            }

            if (i < inputString.Length - 2)
            {
                if ((inputString[i] == '/') 
                    && (((inputString[i + 1] == '0') && (!IsAPeriod(inputString[i + 2]))) 
                    || (!IsAPeriod(inputString[i + 2]))))
                {
                    ErrDivByZero = true;
                }
            }
        }



        if (ErrInvChar) { response += "Expression contains invalid character(s); "; }
        if (ErrInvFirstChar) { response += "Expression should begin with '-', '(' or a digit; "; }
        
        if (ErrMultOperators) { response += "Expression contains two or more operators in a row; "; }
        if (ErrMultPeriods) { response += "Expression contains unexpected '.'"; }

        if (ErrMultZeros) { response += "Expression contains two or more zeros in a row; "; }
        if (ErrNumStartsWithZero) { response += "Numbers that are larger than or equal to 1 cant start with 0; "; }

        if (ErrDivByZero) { response += "Expression contains division by 0; "; }

        if (lBracketsCount > rBracketsCount)
        {
            response += "Expression containts one or more unexpected '('; ";
            isInputValid = false;
        }
        else if (lBracketsCount < rBracketsCount)
        {
            response += "Expression containts one or more unexpected ')'; ";
            isInputValid = false;
        }



        if (response != "") { isInputValid = false;  }

        return isInputValid;
    }

    private void HealInputString()
    {
        inputString = "(" + inputString + ")";

        for (int i = 1; i < inputString.Length; i++)
        {
            if ((IsADigit(inputString[i - 1])) && (inputString[i] == '('))
            {
                inputString.Insert(i, "*");
            }
            if ((IsADigit(inputString[i])) && (inputString[i - 1] == ')'))
            {
                inputString.Insert(i, "*");
            }
            if ((IsAnOperator(inputString[i - 1])) && (inputString[i] == '.'))
            {
                inputString.Insert(i, "0");
            }
        }
    }

    private void ReturnResult()
    {
        if (response == "") { Console.WriteLine("=" + fValue.ToString()); }
        else { Console.WriteLine(response); }
    }

    private void ResetBlocks()
    {
        childBlocks.Clear();
        inputString = "";
        response = "";
    }

    public void Start()
    {
        if (CheckInputString())
        {
            HealInputString();
            CalcBlock();
        }

        ReturnResult();
    }
}

class calc
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter an expression: ");
        MainCalculationBlock MCBlock = new MainCalculationBlock(Console.ReadLine());
        MCBlock.Start();  
    }
}
