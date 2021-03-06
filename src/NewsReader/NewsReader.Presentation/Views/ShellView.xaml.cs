﻿using System.Waf.Presentation.Controls;
using Jbe.NewsReader.Applications.ViewModels;
using Jbe.NewsReader.Applications.Views;
using System;
using System.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jbe.NewsReader.Presentation.Views
{
    [Export(typeof(IShellView)), Shared]
    public sealed partial class ShellView : Page, IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;


        public ShellView()
        {
            InitializeComponent();
            viewModel = new Lazy<ShellViewModel>(() => (ShellViewModel)DataContext);
            Loaded += FirstTimeLoadedHandler;

            contentView.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, ContentChanged);
            previewView.RegisterPropertyChangedCallback(LazyContentPresenter.LazyContentProperty, PreviewChanged);
            UpdatePreviewColumnWidth();

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;
        }


        public ShellViewModel ViewModel => viewModel.Value;


        public void Show()
        {
            Window.Current.Content = this;
            Window.Current.Activate();
        }

        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstTimeLoadedHandler;

            ViewModel.NavigateBackCommand.CanExecuteChanged += NavigateBackCommandCanExecuteChanged;
            UpdateBackButtonVisibility();
        }

        private void ContentChanged(DependencyObject obj, DependencyProperty property)
        {
            navigationSplitView.IsPaneOpen = false;
        }

        private void PreviewChanged(DependencyObject obj, DependencyProperty property)
        {
            UpdatePreviewColumnWidth();
        }

        private void UpdatePreviewColumnWidth()
        {
            previewColumn.Width = previewView.LazyContent == null ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        }

        private void NavigateBackCommandCanExecuteChanged(object sender, EventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        private void UpdateBackButtonVisibility()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ViewModel.NavigateBackCommand.CanExecute(null)
                ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && ViewModel.NavigateBackCommand.CanExecute(null))
            {
                e.Handled = true;
                ViewModel.NavigateBackCommand.Execute(null);
            }
        }
    }
}
