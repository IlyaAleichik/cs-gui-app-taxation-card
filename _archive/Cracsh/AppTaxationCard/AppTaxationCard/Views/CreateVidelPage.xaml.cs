﻿using AppTaxationCard.Models;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AppTaxationCard.Views
{
    public sealed partial class CreateVidelPage : Page
    {
        double la { get; set; }
        double lo { get; set; }
        int KvNum { get; set; }
        int IdKb { get; set; }

        List<MapElement> MyLandmarks = new List<MapElement>();
        List<Forestry> forestries;
        List<TypeEarth> typeEarths;
        List<TypeErrosion> typeErrosions;
        List<DegreeErrosion> degreeErrosions;
        List<ExpositionSlope> expositionSlopes;
        List<TypeOrl> typeOrls;
        List<MaketThenPoroda> maketThenPorodas;

        MaketThen maket;
        Kvartal kvartal;
        SessionUser currentUser;
        Videl videl;

        Account account;
      
        Forestry forestry;
        TypeErrosion typeErrosion;
        DegreeErrosion degree;
        ExpositionSlope exposition;
        TypeOrl typeOrl;
        TypeEarth typeEarth;
        MaketThenPoroda maketThenPoroda;



        public CreateVidelPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                int id = (int)e.Parameter;
                using (ModelContext db = new ModelContext())
                {
                    videl = db.Videls.FirstOrDefault(c => c.Id == id);
                }
            }

            if (videl != null)
            {
                PaneHeader.Text = "Редактирование выдела";
            }
        }
        private void GetDataForGridMaket(ModelContext db)
        {
            SessionUser session = db.SessionUsers.LastOrDefault();
            var dg = db.MaketThens.ToList();
            dataGridMaket.ItemsSource = dg;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (ModelContext db = new ModelContext())
            {              
                forestries = db.Forestries.ToList();
                typeEarths = db.TypeEarths.ToList();
                typeErrosions = db.TypeErrosions.ToList();
                degreeErrosions = db.DegreeErrosions.ToList();
                expositionSlopes = db.ExpositionSlopes.ToList();
                typeOrls = db.TypeOrls.ToList();
                typeOrls = db.TypeOrls.ToList();
                maketThenPorodas = db.MaketThenPorodas.ToList();
            }

            forestryList.ItemsSource = forestries;
            typeEarthList.ItemsSource = typeEarths;
            viewErrosionList.ItemsSource = typeErrosions;
            degreeList.ItemsSource = degreeErrosions;
            expositionList.ItemsSource = expositionSlopes;
            orlList.ItemsSource = typeOrls;
            txtPorodaMaketThen.ItemsSource = maketThenPorodas;
        }


        ObservableCollection<MaketThen> items = new ObservableCollection<MaketThen>();
        private void BtnAddMaketThenItem_Click(object sender, RoutedEventArgs e)
        {
            using (ModelContext db = new ModelContext())
            {
                currentUser = db.SessionUsers.LastOrDefault();
                kvartal = db.Kvartals.Where(x => x.CreateAcc == currentUser.SId).FirstOrDefault(x => x.NumKvartal == KvNum);

                MaketThen maket = new MaketThen
                {
                    Yarus = Convert.ToInt32(txtYarus.Text),
                    Koeficent = Convert.ToInt32(txtKoef.Text),
                    Vozrast = Convert.ToInt32(txtVozrast.Text),
                    Visota = Convert.ToInt32(txtVisota.Text),
                    Diametr = Convert.ToInt32(txtDiametr.Text),

                    MaketThenPoroda = maketThenPoroda,

                    CurrentKvartal = kvartal.Id,
                    CurrentVidel = Convert.ToInt32(numViddels.Text),
                    AccountEditId = currentUser.SId
                };
                items.Add(maket);

            }
            dataGridMaket.ItemsSource = items;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            forestry = forestryList.SelectedItem as Forestry;
            typeErrosion = viewErrosionList.SelectedItem as TypeErrosion;
            degree = degreeList.SelectedItem as DegreeErrosion;
            exposition = expositionList.SelectedItem as ExpositionSlope;
            typeOrl = orlList.SelectedItem as TypeOrl;
            typeEarth = typeEarthList.SelectedItem as TypeEarth;
            maketThenPoroda = txtPorodaMaketThen.SelectedItem as MaketThenPoroda;

            if (forestry == null & typeErrosion == null & degree == null & exposition == null & typeOrl == null & typeEarth == null) return;

            using (ModelContext db = new ModelContext())
            {
                /*db.Kvartals.Where(x => x.CreateAcc == currentUser.SId).FirstOrDefault(x => x.NumKvartal == KvNum);*/

                //var account = db.Kvartals.FirstOrDefault(u => u.NumKvartal == KvNum & u.CreateAcc == currentUser.SId);

                // kvartal = db.Kvartals.FirstOrDefault(u => u.NumKvartal == KvNum & u.CreateAcc == currentUser.SId);

                currentUser = db.SessionUsers.LastOrDefault();


                var kvartal = db.Kvartals.FirstOrDefault(u => u.NumKvartal == KvNum & u.CreateAcc == currentUser.SId);

                if (kvartal != null)
                {
                    if (kvartal.NumKvartal == KvNum)
                    {

                        Crud(db);

                    }
                    else
                    {
                        Kvartal kvartalObj = new Kvartal
                        {

                            NumKvartal = Convert.ToInt32(numKvartal.Text),
                            CreateDateKvartal = DateTime.Now,
                            Latitude = la,
                            Longitude = lo,

                            CreateAcc = currentUser.SId,
                        };

                        db.Kvartals.Add(kvartalObj);
                        db.SaveChanges();

                        Crud(db);

                    }
                }

                Frame.Navigate(typeof(DashboardPage), new DrillInNavigationTransitionInfo());
            }
        }


        private void Crud(ModelContext db)
        {
          

            kvartal = db.Kvartals.LastOrDefault(u => u.CreateAcc == currentUser.SId);
            videl = db.Videls.LastOrDefault(u => u.AccountId == currentUser.SId);

            Videl videlObj = new Videl
            {
                NumVidel = Convert.ToInt32(numViddels.Text),
                Area = Convert.ToInt32(txtArea.Text),
                Krut = Convert.ToInt32(txtCrut.Text),

                Forestry = forestry,
                TypeErrosion = typeErrosion,
                DegreeErrosion = degree,
                ExpositionSlope = exposition,
                TypeOrl = typeOrl,
                TypeEarth = typeEarth,
                CreateDateVidel = DateTime.Now,

                AccountId = currentUser.SId,
                KvartalId = kvartal.Id
            };

            db.Forestries.Attach(forestry);
            db.TypeErrosions.Attach(typeErrosion);
            db.DegreeErrosions.Attach(degree);
            db.ExpositionSlopes.Attach(exposition);
            db.TypeEarths.Attach(typeEarth);
            db.TypeOrls.Attach(typeOrl);

            db.Videls.Add(videlObj);
            db.SaveChanges();


            videl = db.Videls.LastOrDefault(u => u.AccountId == currentUser.SId);
            MaketThen maketObj = new MaketThen
            {
                Yarus = Convert.ToInt32(txtYarus.Text),
                Koeficent = Convert.ToInt32(txtKoef.Text),
                Vozrast = Convert.ToInt32(txtVozrast.Text),
                Visota = Convert.ToInt32(txtVisota.Text),
                Diametr = Convert.ToInt32(txtDiametr.Text),
                Polnota = Convert.ToInt32(txtPolnota.Text),
                MaketThenPoroda = maketThenPoroda,
                AccountEditId = currentUser.SId,
                CurrentKvartal = kvartal.Id,
                CurrentVidel = videl.Id

            };

            db.MaketThenPorodas.Attach(maketThenPoroda);
            db.MaketThens.Add(maketObj);
            db.SaveChanges();
        }






        #region Map Functions
        private async void InitialLocation(object sender, RoutedEventArgs e)
        {

            var accessStatus = await Geolocator.RequestAccessAsync();

            var geolocator = new Geolocator();

            var position = await geolocator.GetGeopositionAsync();

            var mapLocation = await MapLocationFinder.FindLocationsAtAsync(position.Coordinate.Point);

            await Map.TrySetViewAsync(new Geopoint(new BasicGeoposition { Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude }), 16, RotationSlider.Value, PitchSlider.Value, MapAnimationKind.Bow);

            if (mapLocation.Status == MapLocationFinderStatus.Success)
            {
                MyLocation.Text = mapLocation?.Locations?[0].Address.FormattedAddress;
            }
            else
            {
                MyLocation.Text = "Not Found";
            }

            BasicGeoposition snPosition = new BasicGeoposition { Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude };
            Geopoint snPoint = new Geopoint(snPosition);

            var MyLandmarks = new List<MapElement>();

            var spaceNeedleIcon = new MapIcon
            {
                Location = snPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = "Space Needle"
            };

            MyLandmarks.Add(spaceNeedleIcon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = MyLandmarks
            };

            Map.Layers.Add(LandmarksLayer);



            Map.Center = new Geopoint(new BasicGeoposition { Latitude = 52.181427, Longitude = 5.399780 });
            Map.ZoomLevel = 16;
        }
        private void RotationSliderOnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Map.TryRotateToAsync(e.NewValue);

       
            Map.Heading = e.NewValue;

            Map.TryRotateAsync(e.NewValue);
        }

        //private async void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        //{
        //    var resultText = new StringBuilder();

        //    Yay! Goodbye string.Format!
        //    resultText.AppendLine($"Position={args.Position.X},{args.Position.Y}");
        //    resultText.AppendLine($"Location={args.Location.Position.Latitude:F9},{args.Location.Position.Longitude:F9}");

        //    foreach (var mapObject in args.MapElements)
        //    {
        //        resultText.AppendLine("Found: " + mapObject.ReadData<PointLight>().Name);
        //    }
        //    var dialog = new MessageDialog(resultText.ToString(),
        //      args.MapElements.Any() ? "Found the following objects" : "No data found");
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //      async () => await dialog.ShowAsync());
        //}


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            GoToMainPage();
        }

        private void GoToMainPage()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(DashboardPage));
        }

        private void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {

            MyLandmarks.Clear();
            Map.Layers.Clear();
            BasicGeoposition basgeo_edit_position = args.Location.Position;
            la = basgeo_edit_position.Latitude;
            lo = basgeo_edit_position.Longitude;

            Debug.WriteLine("tapped - lat: " + basgeo_edit_position.Latitude.ToString() + "  lon: " + basgeo_edit_position.Longitude.ToString());

            BasicGeoposition snPosition = new BasicGeoposition { Latitude = basgeo_edit_position.Latitude, Longitude = basgeo_edit_position.Longitude };
            Geopoint snPoint = new Geopoint(snPosition);

            var spaceNeedleIcon = new MapIcon
            {
                Location = snPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = "Space Needle"
            };

            MyLandmarks.Add(spaceNeedleIcon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = MyLandmarks
            };

            Map.Layers.Add(LandmarksLayer);
        }

        private void Map_ActualCameraChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapActualCameraChangedEventArgs args)
        {
            var buttons = new Dictionary<Point, Button>();

            foreach (var child in Map.Children)
            {

                var btn = child as Button;
                Point point;
                try
                {
                    point = btn.TransformToVisual(Map).TransformPoint(new Point());
                }
                catch (Exception)
                {
                    return;
                }

                if (point.X > 0 && point.X <= Map.ActualWidth &&
                    point.Y > 0 && point.Y <= Map.ActualHeight)
                {
                    btn.IsTabStop = true;
                    buttons.Add(point, btn);
                }
                else
                {
                    btn.IsTabStop = false;
                }
            }

            Button previosBtn = null;
            var orderedButtons = buttons.OrderBy(b => b.Key.X);

            foreach (var point in orderedButtons)
            {
                var button = point.Value;

                button.XYFocusUp = button;
                button.XYFocusRight = button;
                button.XYFocusLeft = previosBtn != null ? previosBtn : button;
                //button.XYFocusDown = MainControlsViewOldAdventuresButton;

                if (previosBtn != null)
                {
                    previosBtn.XYFocusRight = button;
                }

                previosBtn = button;
            }

            if (orderedButtons.Count() > 1)
            {
                orderedButtons.Last().Value.XYFocusRight = orderedButtons.First().Value;
                orderedButtons.First().Value.XYFocusLeft = orderedButtons.Last().Value;
            }
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            var toolbar = Map.FindDescendant<StackPanel>();
            if (toolbar != null)
            {
                toolbar.Visibility = Visibility.Collapsed;
            }

            var swapchainpanel = Map.FindDescendant<SwapChainPanel>();
            if (swapchainpanel != null && swapchainpanel.Parent != null)
            {
                var grid = swapchainpanel.Parent as Grid;

                var border = new Border()
                {
                    IsHitTestVisible = false,

                    Opacity = 0.4
                };

                grid.Children.Add(border);
            }
        }

        private void EditMapIconPosition(BasicGeoposition basgeo_edit_position)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
