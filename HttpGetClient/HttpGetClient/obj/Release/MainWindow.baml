   M S B A M L   `   `   `  ����   HttpGetClient    HttpGetClient.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  
MainWindow��$	��400��$	��300�� J� 	AllowDrop .  � WindowStartupLocation$ CenterScreen=�.���  � RowDefinitions �� $
��auto ��� $��* ��� $
��auto �.+��� $��0��$��服务器设置��.���  �� ��  � ColumnDefinitions �� $
��auto ��� $
��auto ��� $��* �.+��� $��服务地址:��$��0��$��Right=��� -   $��textBoxServer��$��1��$#��https://192.168.1.120/server/��$	��5,2q�$��2���� $!��上传封包大小(字节):��$��1��$��Right=��� -   $��textUnitSize��$��1��$
��1024��$	��5,2q�$��1��$	��100��$
��Left=��� $��测试��$��1��$	��5,2q�$��2��$
��Left=�$	��100���� $��1��$��服务器文件列表��.���  �� $
��auto ��� $��* � �� �� .+��� -   $��buttonReflash��$��刷新列表��$	��5,5q�$��0���� -   $��buttonDelete��$��删除选中的文件��$	��5,5q�$��1���� -   $��listBoxFiles��$��1��$��2���� -   $��stackPanelFileUpload��$��2��$��5q�y���  �� Path$ 
Visibility�.+��� $��文件上传进度：��$	��0,5q�� $��10�� � Minimum$ 0Q� � Value �� $ 
UploadSize� � Maximum �� $ FileSize�