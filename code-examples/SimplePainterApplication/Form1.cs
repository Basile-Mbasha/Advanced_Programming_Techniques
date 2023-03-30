namespace SimplePainterApplication
{
    public partial class Form1 : Form
    {
        public List<Shape> Shapes = new List<Shape>();
        public List<Shape> RedoStack = new List<Shape>();

        public Form1()
        {
            InitializeComponent();
            strokeColorDialog.Color = Color.Black;
            fillColorDialog.Color = Color.Aquamarine;
            UpdatePanels();

            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;

        }

        private void UpdatePanels()
        {
            panel1.BackColor = strokeColorDialog.Color;
            panel2.BackColor = fillColorDialog.Color;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            strokeColorDialog.ShowDialog();
            UpdatePanels();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fillColorDialog.ShowDialog();
            UpdatePanels();
        }
        
        // TODO: Stop drawing on a simple mouseLeft click
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!isDrawing)
            {
                var point = pictureBox1.PointToScreen(new Point(0, 0));
                var x = MousePosition.X - point.X;
                var y = MousePosition.Y - point.Y;
                Shapes.Add(new Shape()
                {
                    X = x,
                    Y = y,
                    Width = shapeWidthTrackBar.Value,
                    Height = shapeHeightTrackBar.Value,
                    StrokeColor = strokeColorDialog.Color,
                    FillColor = fillColorDialog.Color,
                    Filled = filledCheckBox.Checked,
                    StrokeThickness = strokeWidthTrackBar.Value,
                    Type = ellipseRadioButton.Checked ? ShapeType.Ellipse : ShapeType.Rect,
                });
            }
             pictureBox1.Invalidate();
       }
        
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var shape in Shapes)
            {
                using var pen = new Pen(shape.StrokeColor, shape.StrokeThickness);
                using var brush = new SolidBrush(shape.FillColor);

                if (shape.Type == ShapeType.Ellipse)
                {
                    if (shape.Filled)
                    {
                        e.Graphics.FillEllipse(brush, shape.X, shape.Y, shape.Width, shape.Height);
                    }

                    e.Graphics.DrawEllipse(pen, shape.X, shape.Y, shape.Width, shape.Height);
                }

                if (shape.Type == ShapeType.Rect)
                {
                    if (shape.Filled)
                    {
                        e.Graphics.FillRectangle(brush, shape.X, shape.Y, shape.Width, shape.Height);
                    }

                    e.Graphics.DrawRectangle(pen, shape.X, shape.Y, shape.Width, shape.Height);
                }
            }

            // If currently drawing a shape, draw the current shape
            if (isDrawing && currentShape != null)
            {
                using var pen = new Pen(currentShape.StrokeColor, currentShape.StrokeThickness);
                using var brush = new SolidBrush(currentShape.FillColor);

                if (currentShape.Type == ShapeType.Ellipse)
                {
                    if (currentShape.Filled)
                    {
                        e.Graphics.FillEllipse(brush, currentShape.X, currentShape.Y, currentShape.Width, currentShape.Height);
                    }

                    e.Graphics.DrawEllipse(pen, currentShape.X, currentShape.Y, currentShape.Width, currentShape.Height);
                }

                if (currentShape.Type == ShapeType.Rect)
                {
                    if (currentShape.Filled)
                    {
                        e.Graphics.FillRectangle(brush, currentShape.X, currentShape.Y, currentShape.Width, currentShape.Height);
                    }

                    e.Graphics.DrawRectangle(pen, currentShape.X, currentShape.Y, currentShape.Width, currentShape.Height);
                }
            }
        }


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("Draw five shapes here!");
            
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                int x = random.Next(pictureBox1.Width);
                int y = random.Next(pictureBox1.Height);

                Shapes.Add(new Shape()
                {
                    X = x,
                    Y = y,
                    Width = shapeWidthTrackBar.Value,
                    Height = shapeHeightTrackBar.Value,
                    StrokeColor = strokeColorDialog.Color,
                    FillColor = fillColorDialog.Color,
                    Filled = filledCheckBox.Checked,
                    StrokeThickness = strokeWidthTrackBar.Value,
                    Type = ellipseRadioButton.Checked ? ShapeType.Ellipse : ShapeType.Rect,
                });
            }

            pictureBox1.Invalidate();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Support undo here by deleting the last item in the list");

            // Delete the most recent shape
            if (Shapes.Count > 0)
                Shapes.RemoveAt(Shapes.Count - 1);
            pictureBox1.Invalidate();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("Clear all the shapes here");

            Shapes.Clear();
            pictureBox1.Invalidate();
        }

        // New feature implementation starts here...

        // Initialize needed variables

        private Shape currentShape; // store current coordinates until mouseButton is released
        private bool isDrawing; // check if the drawing process is true and implement feature desired
        private Point startPoint; // store the starting point of the shape being drawn
        private Point endPoint;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing= true;
            // Set the starting point and create a new Shape
            startPoint = e.Location;
            currentShape = new Shape()
            {
                X = e.X,
                Y = e.Y,
                Width = 0,
                Height = 0,
                StrokeColor = strokeColorDialog.Color,
                FillColor = fillColorDialog.Color,
                Filled = filledCheckBox.Checked,
                StrokeThickness = strokeWidthTrackBar.Value,
                Type = ellipseRadioButton.Checked ? ShapeType.Ellipse : ShapeType.Rect,
            };

            // Set the isDrawing flag to true
            isDrawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Check if the mouse button is down and the user is drawing
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                // Calculate the size of the shape being drawn
                currentShape.Width = e.X - currentShape.X;
                currentShape.Height = e.Y - currentShape.Y;
                if (currentShape.Width < 0)
                {
                    currentShape.X = e.X;
                    currentShape.Width *= -1;
                }
                if (currentShape.Height < 0)
                {
                    currentShape.Y = e.Y;
                    currentShape.Height *= -1;
                }

                // Redraw the picture box
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Check if the user is drawing
            if (e.Button == MouseButtons.Left && isDrawing)
            {
                // Set the isDrawing flag to false
                isDrawing = false;

                // Add the new Shape to the list of Shapes
                Shapes.Add(currentShape);
                currentShape = null;

                // Redraw the picture box
                pictureBox1.Invalidate();
            }
        }

    }
}