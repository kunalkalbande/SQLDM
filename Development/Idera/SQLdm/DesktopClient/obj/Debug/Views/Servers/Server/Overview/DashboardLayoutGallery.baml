   M S B A M L   `   `   `  ����N  JSQLdmDesktopClient, Version=11.1.0.0, Culture=neutral, PublicKeyToken=nullo<clr-namespace:Idera.SQLdm.DesktopClient.Views.Servers.Server.Idera.SQLdm.DesktopClient.Views.Servers.Server  5      �Eclr-namespace:Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview7Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview  5      T    NIdera.SQLdm.DesktopClient.Views.Servers.Server.Overview.DashboardLayoutGallery   5   !   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      OServer<clr-namespace:Idera.SQLdm.DesktopClient.Views.Servers.Server     5      ZOverviewEclr-namespace:Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview     5      e aInfragisticsWPF4.v11.1, Version=11.1.20111.2282, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb/	igWindowshttp://infragistics.com/Windows  5      s oInfragisticsWPF4.DataPresenter.v11.1, Version=11.1.20111.2282, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb0igDP%http://infragistics.com/DataPresenter  5      L   FIdera.SQLdm.DesktopClient.Views.Servers.Server.WpfServerDesignBaseView    Loaded$  WpfServerBaseView_Loaded��5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��26 �6   ��5   
   �� 5       �� 
StartPoint$ 0.5,0&�6"    �� EndPoint$ 0.5,1&�65   .��5      �  � Color$ #efefef��6    � Offset$
 0.25Q�6/   � 5      $ #757575��6   $	 1.0Q�6/   5       � 	Resources 5   
      pathHorizontal �� Stretch J� Opacity��%  &	        F� 5      $��pathHorizontal��6   $��0,0,0,0q�5      $
��Left=�5       $	��Top=�5!      $
��Auto��5"      $
��Auto��5#      $
 Fill=�5$      $ 1Q�5%      -����@T� ����@T� ����D����D���	���	#FFB4B4B4$��1��5(      5)      .+�5*   
   8  2Infragistics.Windows.DataPresenter.XamDataCarousel  -   5        $$��dashboardLayoutXamDataCarousel��5*       $��0��5+       $
��Left=�5-       :  4Infragistics.Windows.DataPresenter.DataPresenterBase$	   IsSynchronizedWithCurrentItem	 . $	��Top=�5/       $��0,0,0,0q�50       
   GroupByAreaLocation$

 None=�52   y      
DataSource 5,       ��  �� Path$ Configurations� � DataContext 52   a   ��    FieldSettings 53      6  0Infragistics.Windows.DataPresenter.FieldSettings  54         AutoSizeOptions$	 AllX�6%      AutoSizeScope$ 
AllRecords=�6;      CellClickAction$ SelectRecord=�6V      	AllowEdit�  �Infragistics.Windows.Helpers.NullableConverter`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]$ False 6u    56      :  4Infragistics.Windows.Controls.CarouselPanelNavigator �� 
TargetType �� Property �� Value;  5Infragistics.Windows.DataPresenter.DataRecordCellArea  	ImageCell;  5Infragistics.Windows.DataPresenter.CellValuePresenter �� 
TargetType �� RelativeSource  TextCell �� TextAlignment  NameCell��%�  '        '  t     (��     5M   &   *M� *igDP:XamDataGrid+CardBackground+)&	 �    &	 �    &	 �    �� 58      "  6   .�59      ��  � ��$ Center=�6<   �� 5:       � ��$ Bottom=�6:   5;      �� 5<      "  6   .�5=      ��  � ��
 ���� 63   �� 5>       �@ BackgroundActive
 ���� 69   �� 5?       �@ BackgroundHover
 ���� 68   �� 5H       �@ BackgroundSelected
 ���� 6;   �� 5I       �@ BorderActiveBrush
 ���� 6:   �� 5J       �@ BorderHoverBrush
 ���� 69   �� 5K       �@ BorderSelectedBrush
 ���� 6<   5L      �� 5M      $	 0,1&�6o   $	 0,0&�6~   .��5N      � $ Transparent��6#   $ 0Q�67   � 5O      $ Transparent��6#   $ 1Q�67   5P      �� 5Q      "  6*   .�5R      ��  � �� 5S      �� 5T      "  6.   .R�5U   "   �� $
��Auto��5V   (   $
��Auto��5W   (   ��5U   (   ��  �� *TemplatedParent+$ Content�5W   (   5Y      5[      5\      �� 5]      "  6)   .�5^      ��  � �� 5_      �� 5`      "  6.   .R�5a   "   �� 
������ $��4(�5c   ,   $��Arial)�5d   ,   ���#757575$��0,1,0,0q�5f   ,   $ Center=�5g   ,   ��5a   ,   ��  �� *TemplatedParent+$ Content�5g   ,   5i      5k      �� 5l       � ��$ 8��6/   5m      �� 5n      "  6)   .�5o      ��  � �� 5p      �� 5q      "  6.   .R�5r   "   �� 
������ $��14��5t   ,   $��8(�5u   ,   $��Arial)�5v   ,   ���#757575$��0,2,0,0q�5x   ,   $ Center=�5y   ,   ��5r   ,   ��  �� *TemplatedParent+$ Content�5y   ,   5{      5}      �� 5~       � ��$ 14��6/   5      5�         ViewSettings 5�      8	  2Infragistics.Windows.Controls.CarouselViewSettings	  5�      ! 	  AutoScaleItemContentsToFit .  	  IsListContinuous .  	  ItemPathAutoPad .   	  ItemPath# [  5�   1    	  ItemPathPadding$ 
0,-20,0,30q�5�   1    	  ItemPathPrefixPercent$
 0.15Q�5�   1     	  ItemPathSuffixPercent$
  0.15Q�5�   1   ! 	  ItemsPerPage$! 3��5�   1   !" 	  ReserveSpaceForReflections" . !# 	  ScalingEffectStopDirection$# 
Horizontal=�5�   1   $ 	  
UseOpacity$ . % 	  
UseScaling% . & 	  
UseSkewing& . ' 	  	UseZOrder' . ( 	  OpacityEffectStops( 5�      ?
  9Infragistics.Windows.Controls.OpacityEffectStopCollection
  5  /Infragistics.Windows.Controls.OpacityEffectStop  5�      .  (Infragistics.Windows.Controls.EffectStop)   Offset$) 0Q�6:   *   Value$	* 0.5Q�6E     5�      $
) 0.45Q�6:   $	* 0.5Q�6H     5�      $) .5Q�6:   $* 1Q�6F     5�      $
) 0.55Q�6:   $	* 0.5Q�6H     5�      $) 1Q�6:   $	* 0.5Q�6E   5�      + 	  ScalingEffectStops+ 5�      ?  9Infragistics.Windows.Controls.ScalingEffectStopCollection  5  /Infragistics.Windows.Controls.ScalingEffectStop  5�      $) 0Q�6:   $* 2Q�6E     5�      $
) 0.45Q�6:   $* 2Q�6H     5�      $	) 0.5Q�6:   $	* 5.5Q�6G     5�      $
) 0.55Q�6:   $* 2Q�6H     5�      $) 1Q�6:   $* 2Q�6E   5�      , 	  SkewAngleYEffectStops, 5�      B  <Infragistics.Windows.Controls.SkewAngleYEffectStopCollection  8  2Infragistics.Windows.Controls.SkewAngleYEffectStop  5�      $
) 0.15Q�6=   $	* -20Q�6K     5�      $
) 0.45Q�6=   $	* -20Q�6K     5�      $) .5Q�6=   $* 0Q�6I     5�      $
) 0.55Q�6=   $* 20Q�6K     5�      $
) 0.85Q�6=   $* 20Q�6K   5�      - 	  ZOrderEffectStops- 5�      4  .Infragistics.Windows.Controls.ZOrderEffectStop  $) 0Q�65   $* 0Q�6@     5�      $	) 0.5Q�65   $* 1Q�6B     5�      $) 1Q�65   $* 0Q�6@   5�      .   FieldLayoutSettings. 5�      <  6Infragistics.Windows.DataPresenter.FieldLayoutSettings  5�      /   AutoGenerateFields$/ FalseY�6+   0   AutoArrangeCells$0 Never=�5�   +    1   HighlightAlternateRecords$1 FalseY�5�   +   2   HighlightPrimaryField$2 SameAsOtherFields=�5�   +   3   SelectionTypeRecord$3 SingleAutoDrag=�5�   +   4   DataRecordSizingMode$4 SizedToContentAndFixed=�6P   $5   ExpansionIndicatorDisplayMode$5 Never=�6~   6   FilterUIType$6 Default=�6�   7   MaxSelectedRecords$7 1X�6�   8   FieldLayouts8 5�      4  .Infragistics.Windows.DataPresenter.FieldLayout  5�      9   Settings9 5�        5�      ):   "AutoArrangePrimaryFieldReservation$
: None=�63   ;   AutoFitMode$; Always=�6]   $4 SizedToContentAndFixed=�6r   $7 1X�6�   <   SelectionTypeCell$
< None=�6�   =   SelectionTypeField$
= None=�6�   $3 Single=�6�   >   DefaultRowDefinition> 5�      �� 5�   "   5�      ?   FieldSettings? 5�        5�      $ SelectRecord=�6-   @   CellContentAlignment$@ 	ValueOnly=�6L   A   FieldsA 5�      .  (Infragistics.Windows.DataPresenter.Field  5�      B   Name$B 
ImageBytes��6%   
C   Row$C 0��67   D   IsExpandable$D FalseY�6?   E   	IsPrimaryE . F   SettingsF 5�        5�   "   G   CellValuePresenterStyle#G [ 65   5�        5�      $B Owner��6%   $C 1��62   $D FalseY�6:   F 5�        5�   "   #G [ 65   5�        5�      $
B Name��6%   $C 2��61   $D FalseY�69   F 5�        5�   "   #G [ 65   5�        5�      $B LastViewedText��6%   $C 3��6;   $D FalseY�6C   F 5�        5�   "   #G [ 65   5�        5�      $B LastUpdatedText��6%   $C 4��6<   $D FalseY�6D   F 5�        5�   "   #G [ 65   5�        5�      $B 	RowNumber��6%   $C 5��66   H   
Visibility$H 	Collapsed=�6>   5�      5�      �� 5�   
   $��1��6   $��Bottom=�6!   $ Center=�6<   ���#454545$��14(�6�   $
��Bold"�6�   ��6S   �� $ SelectionText�6�   5�      5�      