   M S B A M L   `   `   `  ����]  YDevExpress.Data.v13.2, Version=13.2.9.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aP<clr-namespace:DevExpress.Data;assembly=DevExpress.Data.v13.2DevExpress.Data  5	   	    Thermoformer   Thermoformer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   a ]DevExpress.Xpf.Grid.v13.2, Version=13.2.9.0, Culture=neutral, PublicKeyToken=b88d1754d700e49af bDevExpress.Xpf.Grid.v13.2.Core, Version=13.2.9.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   5   	   a ]DevExpress.Xpf.Core.v13.2, Version=13.2.9.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   ;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   GData<clr-namespace:DevExpress.Data;assembly=DevExpress.Data.v13.2  5	   	   -   5          �Name$  myWindow��5   	    � Title$# ThermoFormer Calibration Tool��5   	   $	��600��6/   $	��610��6<   $��Center=�6H   .��5      �� $
��Auto=�6   .��5   
   � -   5         �Name$ 
myTestGrid��5      
�������.+�5      ]� -   5         ]�Name$ dockPanelOuterContainer��5      
�������$	��Top=�6R   .+�5      ]� -   5        $! HorizontalBlockForPageTitle��5      
�������$��40��6Y   $	��Top=�6e   $	��Top=�6}   .+�5      �� -   5         ��Name$ pageHeaderTextBox��5      $��60��69   $#��ThermoFormer Calibration Tool��6E    �� TextAlignment$ Center=�6j   
�������$��20(�6�   $
��Bold"�6�   5      ]� 5      -   5        $$ HorizontalBlockForGridElements��5      
�������$	��Top=�6\   $	��Top=�6t   .+�5      ]� -   5        $ 
LeftSpacer��5       
�������$��10��6L   $
��Left=�6X   5      � 5      .+�5      &   System.Windows.Controls.DataGrid  -   5          Name$ boundDataGrid��5   '      CanUserSortColumns .     HorizontalGridLinesBrush �	#FF2A57B2	   VerticalGridLinesBrush	 �	#FF2A57B2���	#FFB7CFED��.  $��Hidden=�5      $��Hidden=�5       
   CanUserReorderColumns
 .  ��5      �� *ListOfDataTableRows+��5"   "   �� 5#   &    �� 
TargetType"  6,   .�5$   *   ��  �� Property  �@ AlternatingRowBackground �� Value �	#FFB1B6BB6U   5%   '   5'      5(      ]� 5,      -	   5        $ dockPanel_MainDockpanel��5,   $   $	��Top=�6E   $
��Left=�6]   5-      ]� 50      -
   5        $ RightSpacer��50       
�������$��10��6M   $
��Left=�6Y   51      52      ]� 54      -   5        $ BottomSpacer��54      
����ح�$��25��6J   $
��Left=�6X   $��0q�6n   �� 56      -   5         ��Name$ SaveAndQuitButton��56      $	��131��64   $��44��6@   $��Save and Quit��6L   $��
100,10,0,0q�6d   $
��Left=�6x   �� 57      -   5        $ Cancel��57      $	��131��6)   $��44��65   $��Cancel��6A   $��
100,10,0,0q�6R   $
��Left=�6f   58      59      5:      5;      