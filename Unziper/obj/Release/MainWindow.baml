   M S B A M L   `   `   `  ���� B  >Unziper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:UnziperUnziper      Unziper.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:Unziper       � Title$  Unziper��$	��597��$��1111.333�� � Icon$ FolderClosed.ico�����	#FF5FB7E0���	#FF58B4CF���	#FF9AE1EC J� OpacityMask �	#FF519DD3 � WindowStyle$ ThreeDBorderWindow=� � WindowStartupLocation$ CenterScreen=�.��� $��0,0,0,4q�.+�]� $��53����.  $��
10,10,10,0q�$	��Top=� J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~��� �� �� T� .+��� $��53��$	��Top=�$	��103��$ 0,0.5&�.+��� -    ��Name$ label��$��Source folder:��$	��Top=�$��26��$��Trebuchet MS)�$��14(���Z� �� -   $ 
label_Copy��$��Target folder:��$	��Top=�$��26��$��Trebuchet MS)�$��14(���Z� �� $��53��$	��Top=�$	��583��$ 0.5,0.5&�z�V� .~��� �� �� T� .+��� -    ��Name$ sourceTextBox��$��25�� �� TextWrapping$
 Wrap=�$�� ��$	��Top=��� -   $ targetTextBox��$��25��$
 Wrap=�$�� ��$	��Top=�$
��Left=�$	��583��	 �� 
IsReadOnly	 . �� $��53��$	��Top=�$��87��.+��� -   
 ��Name$
 getListButton��$��Get list��$��25��$	��Top=�$��10,0,0,0q� � ToolTip$% Get files list in Source folder���� -   $
 targetButton��$��Browse��$��25��$	��Top=�$��10,0,0,0q�$ Select Target folder���� $��53��$	��Top=�$��97��.+��� -   $
 
copyButton��$
��Copy��$��25��$	��Top=����	#FF07C2EC$
��10,0q�$# Copy selected files to Target��{�.  �� -   $
 unzipButton��$��Unzip��$��25��$	��Top=�$ 0.556,-0.75&����	#FF07C2EC$
��10,0q�$&  Unzip all files in Target folder��{�.  �� $	��130��.+��� -	    ��Name$ autoUnzipCheckBox��$��Unzip after copy��$��0,5,0,5q�$��1q� J� Opacity$	 0.8Q����	#FF032E7A
���   �$��16��$@ :Automatically unzip all files in Target folder after copy ����Z� �� -
   $ autoDeleteCheckBox��$��Delete after extract��$��0,5,0,5q�$��1q�$	 0.8Q����	#FF032E7A
���   �$��22��$@ :Automatically delete zip in Target folder after extracting����Z� �� $��69��.+��� -   $
 cancelButton��$��Cancel��$��50��$	��Top=�$��14(�{�. � -    �Name$ ListGrid�� � ShowGridLines .  $��10,68,-2,10q�$ 0.442,0.502&� � ColumnDefinitions �� �� .+��� $��0��
�������$��Right=�$��2��$��0,0,0,27q��� -    ��Name$ sourceListView��$ �� IsSynchronizedWithCurrentItem$
 TrueY�$��0,7,7,27q� �� SelectionMode$ Single=�|�.  %� IsVirtualizing .  ���� *
sourceList+ �� View �� -   gridView��P���  �� HeaderTemplate �� .R��� -    �� CellTemplate �� .R��� $��listChekBox��-    d� IsThreeState .  |�.  d� 	IsChecked �� *	IsChecked+
 � Tag �� *Id+��  �� .R��� $��16��$��16��|�.  ���� *
FileIcon+��  �� Width$
 Auto�� �� DisplayMemberBinding ��  �� Path$
 Name�.��	File name�� -   $ filesListView��$ Single=�$��	5,7,10,27q�$ 0.5,0.5&�$
 TrueY����	#FFCBF9FD}�. $��1�� ��  �� AllowsColumnReorder .  .P��� -   actionColumn��$��Action��� -    �Name$ progressBar��$��22��$��Bottom=�$ Copy progress�����	#FF858785$
 0.80Q�  � LargeChange$  0Q�$y�Hidden=��� $��1��$
��Left=�$��10,0,0,0q�$	��471��$��22��$��Bottom=�.+��� -   $ timeLeft��$�� ��$��11(�$��22��$��Bottom=�$y�Hidden=�