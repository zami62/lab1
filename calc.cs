using System;
using System.Collections.Generic;
using System.Linq;

public static class Characters
{   
    public static char[] availableDigits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    public static char[] availableOperators = new char[] { '+', '-', '*', '/' };
    public static char[] availableBrackets = new char[] { '(', ')' };

    public static bool IsADigit(char ch)
    {
        if (availableDigits.Contains(ch)) { return true; }
        else { return false; }
    }

    public static bool IsAnOperator(char ch)
    {
        if (availableOperators.Contains(ch)) { return true; }
        else { return false; }
    }

    public static bool IsABracket(char ch)
    {
        if (availableBrackets.Contains(ch)) { return true; }
        else { return false; }
    }

    public static bool IsAPeriod(char ch)
    {
        if (ch == '.') { return true; }
        else { return false; }
    }

    public static bool IsAValidFirstChar(char ch)
    {
        if ((ch == '-') || (ch == '(') || (ch == '.') || IsADigit(ch)) { return true; }
        else { return false; }
    }

    public static bool IsAValidLastChar(char ch)
    {
        if (IsADigit(ch) || (ch == ')')) { return true; }
        else { return false; }
    }

}

public class CalculationBlock
{
    protected List<CalculationBlock> childBlocks = new List<CalculationBlock>();

    protected string inputString = "";

    private int leftBracketsCount = 0;
    private int rightBracketsCount = 0;

    protected string sValue = "";
    protected float fValue = 0;

    public CalculationBlock() { }

    public CalculationBlock(string sValue)
    {
        this.sValue = sValue;
    }


    private void DivideIntoBlocks()
    {
        if (inputString != "")
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
                    if ((Characters.IsADigit(inputString[i])) || (inputString[i] == '.'))
                    {
                        value += inputString[i];
                    }
                    else if (Characters.IsAnOperator(inputString[i]))
                    {
                        if (value != "")
                        {
                            childBlocks.Add(new CalculationBlock(value));
                            value = "";
                        }

                        childBlocks.Add(new CalculationBlock(inputString[i].ToString()));
                    }

                    if (Characters.IsABracket(inputString[i + 1]))
                    {
                        if (value != "")
                        {
                            childBlocks.Add(new CalculationBlock(value));
                            value = "";
                        }
                    }
                }

                if ((inputString[i] == ')')) { rightBracketsCount++; }

                if ((leftBracketsCount == rightBracketsCount) && (bracketsBlock.inputString != "")) 
                {
                    childBlocks.Add(bracketsBlock);
                    bracketsBlock = new CalculationBlock();
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

        if ((childBlocks.Count == 0) && (Characters.IsADigit(sValue.Last())))
        {
            fValue = float.Parse(sValue);
        }
    }

    private void CalcChildBlocks()
    {
        foreach (CalculationBlock child in childBlocks)
        {
            child.CalcBlock();
        }
    }

    private void CalcSelf()
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
        DivideIntoBlocks();

        CastChildBlocks();

        CalcChildBlocks();

        CalcSelf();

        if (childBlocks.Count != 0) { fValue = childBlocks[0].fValue; }
    }
}

public class MainCalculationBlock : CalculationBlock
{
    private string response = "";

    public MainCalculationBlock() { }

    private void GetInput()
    {
        Console.WriteLine("Enter an expression: ");
        inputString = Console.ReadLine();
    }

    private bool CheckInputString()
    {
        bool isInputValid = true;

        bool ErrInvChar = false;
        bool ErrInvFirstChar = false;
        bool ErrInvLastChar = false;

        bool ErrInvBlockFirstChar = false;
        bool ErrInvBlockLastChar = false;

        bool ErrMultOperators = false;
        bool ErrMultPeriods = false;
        bool ErrMultZeros = false;

        bool ErrNumStartsWithZero = false;
        bool ErrNumEndsWithPeriod = false;

        bool ErrDivByZero = false;

        int lBracketsCount = 0;
        int rBracketsCount = 0;


        if (!Characters.IsAValidFirstChar(inputString[0]))
        {
            ErrInvFirstChar = true;
        }

        if (!Characters.IsAValidLastChar(inputString.Last()))
        {
            ErrInvLastChar = true;
        }

        for (int i = 0; i < inputString.Length; i++)
        {
            if (inputString[i] == '(') { lBracketsCount++; }
            else if (inputString[i] == ')') { rBracketsCount++; }

            if (!(Characters.IsADigit(inputString[i]) 
                || Characters.IsAnOperator(inputString[i]) 
                || Characters.IsABracket(inputString[i]) 
                || Characters.IsAPeriod(inputString[i])))
            {
                ErrInvChar = true;        
            }

            if (i < inputString.Length - 1)
            {
                if ((Characters.IsAnOperator(inputString[i])) && (Characters.IsAnOperator(inputString[i + 1])))
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

                if ((inputString[i] == '0') && Characters.IsADigit(inputString[i + 1]))
                {
                    ErrNumStartsWithZero = true;
                }
                if ((inputString[i] == '.') && !Characters.IsADigit(inputString[i + 1]))
                {
                    ErrNumEndsWithPeriod = true;
                }

                if ((inputString[i] == '(') && !Characters.IsAValidFirstChar(inputString[i + 1]))
                {
                    ErrInvBlockFirstChar = true;
                }
                if (!Characters.IsAValidLastChar(inputString[i]) && (inputString[i + 1] == ')'))
                {
                    ErrInvBlockLastChar = true;
                }
            }

            if (i < inputString.Length - 2)
            {
                if ((inputString[i] == '/') 
                    && (((inputString[i + 1] == '0') 
                    && (!Characters.IsAPeriod(inputString[i + 2])))))
                {
                    ErrDivByZero = true;
                }
            }
        }


        if (ErrInvChar) { response += "Expression contains invalid character(s); "; }

        if (ErrInvFirstChar) { response += "Expression should begin with '-', '(' or a digit; "; }
        if (ErrInvLastChar) { response += "Expression should end with ')' or a digit; "; }

        if (ErrInvBlockFirstChar) { response += "Parenteses should begin with '-', '(' or a digit; "; }
        if (ErrInvBlockLastChar) { response += "Parenteses should end with ')' or a digit; "; }

        if (ErrMultOperators) { response += "Expression contains two or more operators in a row; "; }
        if (ErrMultPeriods) { response += "Expression contains two or more periods in a row"; }
        if (ErrMultZeros) { response += "Expression contains two or more zeros in a row; "; }

        if (ErrNumStartsWithZero) { response += "Numbers that are larger than or equal to 1 cant start with 0; "; }
        if (ErrNumEndsWithPeriod) { response += "Number cant end with a period; "; }

        if (ErrDivByZero) { response += "Expression contains division by 0; "; }

        if (lBracketsCount > rBracketsCount) { response += "Expression containts one or more unexpected '('; "; }
        else if (lBracketsCount < rBracketsCount) { response += "Expression containts one or more unexpected ')'; "; }


        if (response != "") { isInputValid = false;  }

        return isInputValid;
    }

    private void HealInputString()
    {
        for (int i = 0; i < inputString.Length - 1; i++)
        {            
            if ((Characters.IsADigit(inputString[i])) && (inputString[i + 1] == '('))
            {
                inputString = inputString.Insert(i + 1, "*");        
            }
            else if ((inputString[i] == ')') && (Characters.IsADigit(inputString[i + 1]) || (inputString[i + 1] == '.')))
            {
                inputString = inputString.Insert(i + 1, "*");
            }
            else if ((inputString[i] == ')') && (inputString[i + 1] == '('))
            {
                inputString = inputString.Insert(i + 1, "*");
            }

            if ((Characters.IsAnOperator(inputString[i])) && (inputString[i + 1] == '.'))
            {
                inputString = inputString.Insert(i + 1, "0");
            }
            else if ((inputString[i] == '(') && (inputString[i + 1] == '.'))
            {
                inputString = inputString.Insert(i + 1, "0");
            }
        }

        inputString = "(" + inputString + ")";
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
        fValue = 0;
        sValue = "";
        response = "";
    }

    public void Start()
    {
        GetInput();

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
        MainCalculationBlock MCBlock = new MainCalculationBlock();
        MCBlock.Start();  
    }
}
