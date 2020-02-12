using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testgm
{
    class GameModel
    {
        bool[,] game;
        public readonly int Size;
        public GameModel(int size)
        {
            Size = size;
            game = new bool[size, size];
        }

        public bool this[int row, int column]
        {
            get { return game[row, column]; }
        }
        public event Action<int, int, bool> StateChanged;
        void SetState(int row, int column, bool state)
        {
            game[row, column] = state;
            if (StateChanged != null) StateChanged(row, column, game[row,column]);
        }
        public void Start()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int column = 0; column < Size; column++)
                {
                    SetState(row, column, (row + column) % 2 == 0);
                }
            }
        }
        void Flip(int row, int column)
        {
            SetState(row,column,!game[row,column]);
        }
        public void MakeMove(int row, int column)
        {
            for (int iRow = 0; iRow < Size; iRow++)
            {
                if (iRow == row)
                    Flip(iRow, column);
            }
            for (int iColumn = 0; iColumn < Size; iColumn++)
            {
                if (iColumn == column)
                    Flip(row, iColumn);
            }
            Flip(row, column);
        }
    }
    class MyForm : Form
    {
        TableLayoutPanel table;
        public MyForm(GameModel model)
        {
            table = new TableLayoutPanel { Dock = DockStyle.Fill };
            for (int i = 0; i < model.Size; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / model.Size));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / model.Size));
            }

            for (var column = 0; column < model.Size; column++)
            {
                for (var row = 0; row < model.Size; row++)
                {
                    var button = new Button
                    {
                        Dock = DockStyle.Fill
                    };
                    var iRow = row;
                    var iColumn = column;
                    button.Click += (sender, args) => model.MakeMove(iRow, iColumn);
                    table.Controls.Add(button, column, row);
                }
            }
            model.StateChanged += (row,column,state) => table.GetControlFromPosition(column, row).BackColor = state ? Color.Black : Color.White;
            model.Start();
            Controls.Add(table);

        }
    }
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var model = new GameModel(5);
            var form = new MyForm(model);
            Application.Run(form);
        }
    }
}
