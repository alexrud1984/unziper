   M S B A M L   `   `   `  ����B  >Unziper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:UnziperUnziper  5   	       Unziper.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Unziper     5   	     � Title$  Unziper��5   	   $	��597��6   $��1111.333��6&    � Icon$ FolderClosed.ico��67   ���	#FF5FB7E0���	#FF58B4CF���	#FF9AE1EC J� OpacityMask �	#FF519DD3 � WindowStyle$ ThreeDBorderWindow=�6�    � WindowStartupLocation$ CenterScreen=�6�   .��5	      � $��0,0,0,4q�6   .+�5
   
   ]� $��53��6   ��.  $��
10,10,10,0q�66   $	��Top=�6J    J� RenderTransformOrigin$ 0.5,0.5&�6b   z�5      V� 5      .~�5      �� �� 5      �� 5      T� 5      5      .+�5      �� $��53��6   $	��Top=�6%   $	��103��6=   $ 0,0.5&�6I   .+�5      �� -   5         ��Name$ label��5      $��Source folder:��6'   $	��Top=�6@   $��26��6X   $��Trebuchet MS)�6z   $��14(�6�   ��6d   Z� 6�   �� 5      -   5        $ 
label_Copy��5      $��Target folder:��6,   $	��Top=�6E   $��26��6]   $��Trebuchet MS)�6   $��14(�6�   ��6i   Z� 6�   5      �� 5      $��53��6   $	��Top=�6%   $	��583��6=   $ 0.5,0.5&�6I   z�5      V� 5      .~�5      �� �� 5      �� 5      T� 5      5      .+�5       �� -   5         ��Name$ sourceTextBox��5       $��25��61    �� TextWrapping$
 Wrap=�6=   $�� ��6Q   $	��Top=�6Y   6q   �� 5!      -   5        $ targetTextBox��5!      $��25��61   $
 Wrap=�6=   $�� ��6Q   $	��Top=�6Y   $
��Left=�6q   $	��583��6�   	 �� 
IsReadOnly	 . 6�   5"      �� 5#      $��53��6   $	��Top=�6%   $��87��6=   .+�5$      �� -   5        
 ��Name$
 getListButton��5$      $��Get list��60   $��25��6C   $	��Top=�6O   $��10,0,0,0q�6�    � ToolTip$% Get files list in Source folder��6�   �� 5%      -   5        $
 targetButton��5%      $��Browse��6/   $��25��6@   $	��Top=�6L   $��10,0,0,0q�6   $ Select Target folder��6�   5&      �� 5'      $��53��6   $	��Top=�6%   $��97��6=   .+�5(      �� -   5        $
 
copyButton��5(      $
��Copy��6-   $��25��6<   $	��Top=�6H   ���	#FF07C2EC$
��10,0q�6�   $# Copy selected files to Target��6�   {�.  6�   �� 5)      -   5        $
 unzipButton��5)      $��Unzip��6.   $��25��6>   $	��Top=�6J   $ 0.556,-0.75&�6|   ���	#FF07C2EC$
��10,0q�6�   $&  Unzip all files in Target folder��6�   {�.  6�   5*      �� 5+      $	��130��6   .+�5,      �� -	   5         ��Name$ autoUnzipCheckBox��5,      $��Unzip after copy��66   $��0,5,0,5q�6Q   $��1q�6b    J� Opacity$	 0.8Q�6v   ���	#FF032E7A
���   �$��16��6�   $@ :Automatically unzip all files in Target folder after copy ��6�   ��6�   Z� 6�   �� 5-      -
   5        $ autoDeleteCheckBox��5-      $��Delete after extract��67   $��0,5,0,5q�6V   $��1q�6g   $	 0.8Q�6{   ���	#FF032E7A
���   �$��22��6�   $@ :Automatically delete zip in Target folder after extracting��6�   ��6�   Z� 68  5.      �� 5/      $��69��6   .+�50      �� -   5        $
 cancelButton��50      $��Cancel��6/   $��50��6@   $	��Top=�6L   $��14(�6y   {�. 6�   51      53      � 54   
   -   5         �Name$ ListGrid��54       � ShowGridLines .  $��10,68,-2,10q�67   $ 0.442,0.502&�6L    � ColumnDefinitions 55      �� 56      �� 57      .+�59      �� $��0��6   
�������$��Right=�6=   $��2��6Y   $��0,0,0,27q�6c   �� 5:      -   5         ��Name$ sourceListView��5:      $ �� IsSynchronizedWithCurrentItem$
 TrueY�5;      $��0,7,7,27q�5=       �� SelectionMode$ Single=�5>      |�.  %� IsVirtualizing .  ��5<      �� *
sourceList+ �� View 5E      �� 5F      -   5        gridView��P�5G      �� 5H       �� HeaderTemplate 5I   "   �� 5J   &   .R�5K   *   �� -   5        5L   3   5M   '    �� CellTemplate 5O   "   �� 5P   &   .R�5Q   *   �� $��listChekBox��5R   -   -   5         d� IsThreeState .  |�.  d� 	IsChecked 5S   -   �� *	IsChecked+
 � Tag 5W   -   �� *Id+5X   -   5Z   '   5\      �� 5]       5^   "   �� 5_   &   .R�5`   *   �� $��16��60   $��16��6;   |�.  ��6G   �� *
FileIcon+6c   5a   '   5c      �� 5d       �� Width$
 Auto��5f   !    �� DisplayMemberBinding 5e   !   ��  �� Path$
 Name�.��5f   .   	File name5g      5i      5k      �� 5l      -   5        $ filesListView��5l      $ Single=�6.   $��	5,7,10,27q�6E   $ 0.5,0.5&�6X   $
 TrueY�6x   ���	#FFCBF9FD}�. $��1��6�    5m      �� 5n       �� AllowsColumnReorder .  .P�5o      �� -   5        actionColumn��$��Action��5o   ?   5p      5r      � 5s      -   5         �Name$ progressBar��5s      $��22��6/   $��Bottom=�6;   $ Copy progress��6V   ���	#FF858785$
 0.80Q�6�     � LargeChange$  0Q�6�   $y�Hidden=�6�   �� 5t      $��1��6   $
��Left=�6)   $��10,0,0,0q�6D   $	��471��6V   $��22��6b   $��Bottom=�6n   .+�5u      �� -   5        $ timeLeft��5u      $�� ��6*   $��11(�65   $��22��6C   $��Bottom=�6O   $y�Hidden=�6j   5v      5w      5x      5y      