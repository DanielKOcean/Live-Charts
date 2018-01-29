﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LiveCharts.Core.Abstractions;
using LiveCharts.Core.Abstractions.DataSeries;
using LiveCharts.Core.Coordinates;
using LiveCharts.Core.Data;
using LiveCharts.Core.ViewModels;

namespace LiveCharts.Wpf.PointViews
{
    /// <summary>
    /// The column point view.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TPoint">the type of the chart point.</typeparam>
    /// <typeparam name="TShape">the type of the shape.</typeparam>
    /// <typeparam name="TLabel">the type of the label.</typeparam>
    /// <typeparam name="TCoordinate">the type of the coordinate.</typeparam>
    /// <typeparam name="TViewModel">the type of the view model.</typeparam>
    /// <seealso cref="PointView{TModel, Point,Point2D, ColumnViewModel, TShape, TLabel}" />
    public class ColumnPointView<TModel, TPoint, TCoordinate, TViewModel, TShape, TLabel>
        : PointView<TModel, TPoint, TCoordinate, TViewModel, TShape, TLabel>
        where TPoint : Point<TModel, TCoordinate, TViewModel>, new()
        where TCoordinate : Point2D
        where TViewModel : ColumnViewModel
        where TShape : Shape, new()
        where TLabel : FrameworkElement, IDataLabelControl, new()
    {
        /// <inheritdoc />
        protected override void OnDraw(TPoint point, TPoint previous, IChartView chart, TViewModel viewModel)
        {
            var isNew = Shape == null;

            if (isNew)
            {
                var wpfChart = (CartesianChart) chart;
                Shape = new TShape();
                var r = Shape as Rectangle;
                if (r != null)
                {
                    var radius = (point.Series as IColumnSeries)?.PointCornerRadius ?? 0;
                    r.RadiusY = radius;
                    r.RadiusX = radius;
                }
                wpfChart.DrawArea.Children.Add(Shape);
                Canvas.SetLeft(Shape, viewModel.Left);
                Canvas.SetTop(Shape, viewModel.Zero);
                Shape.Width = viewModel.Width;
                Shape.Height = 0;
            }

            Shape.Stroke = point.Series.Stroke.AsSolidColorBrush();
            Shape.Fill = point.Series.Fill.AsSolidColorBrush();

            var speed = chart.AnimationsSpeed;

            Shape.AsStoryboardTarget()
                .AtSpeed(speed)
                .Property(Canvas.LeftProperty, viewModel.Left)
                .Property(Canvas.TopProperty, viewModel.Top)
                .Property(FrameworkElement.WidthProperty, viewModel.Width)
                .Property(FrameworkElement.HeightProperty, viewModel.Height)
                .Begin();
        }

        /// <inheritdoc />
        protected override void OnDrawLabel(TPoint point, Core.Drawing.Point location, IChartView chart)
        {
            var isNew = Label == null;

            if (isNew)
            {
                var wpfChart = (CartesianChart) chart;
                Label = new TLabel();
                Label.Measure(point.PackAll());
                Canvas.SetLeft(Shape, Canvas.GetLeft(Shape));
                Canvas.SetTop(Shape, Canvas.GetTop(Shape));
                wpfChart.DrawArea.Children.Add((UIElement) Label);
            }

            var speed = chart.AnimationsSpeed;

            ((FrameworkElement) Label).BeginAnimation(
                Canvas.LeftProperty,
                new DoubleAnimation(location.X, speed));
            ((FrameworkElement) Label).BeginAnimation(
                Canvas.TopProperty,
                new DoubleAnimation(location.Y, speed));
        }

        /// <inheritdoc />
        protected override void OnDispose(IChartView chart)
        {
            var wpfChart = (CartesianChart) chart;
            wpfChart.DrawArea.Children.Remove(Shape);
            wpfChart.DrawArea.Children.Remove((UIElement) Label);
        }
    }
}