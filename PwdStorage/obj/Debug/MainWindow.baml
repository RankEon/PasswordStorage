   M S B A M L   `   `   `  ����E  APwdStorage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:PwdStorage
PwdStorage  5   	       PwdStorage.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:PwdStorage     5   	     � Title$  Password Storage��5   	   $��330.778��6"   $��529.805��63    � WindowStartupLocation$ CenterScreen=�6C    � DataContext 5	         PwdStorage.CredentialInfo  5
   
   .��5      � -   5         �Name$ gridMainWindowContent��5      .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGridPasswords��5      $
��Left=�6.   $��	10,44,0,0q�6I   $	��Top=�6\   $	��213��6t   $	��497��6�      CanUserResizeRows .     AutoGenerateColumns .  ��6�   ��  �� Mode$ OneWay=�   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Site��6%   
   HeaderStringFormat$

 Site��63      MaxWidth$	 300Q�6M      MinWidth$	 150Q�6\   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6k   �� *Site+  5      $	 Username��6%   $
 Username��67   $	 300Q�6U   $	 200Q�6d    6s   �� *
Username+  5      $	 Password��6%   $
 Password��67   $	 150Q�6U   $	 100Q�6d    6s   �� *
Password+5      �� 5   
   -   5         ��Name$ btnAddNewPassword��5      $��+ Add��6,   $
��Left=�6<   $��
10,262,0,0q�6W   $	��Top=�6k   $��75��6�   6�   o� 5   
   -   5         o�Name$
 menu��5      $
��Left=�6   $��20��68   $	��Top=�6D   $	��522��6\   }�. .@�5      m� $
��File��6   .@�5      m� -   5         m�Name$ menuItemFileSave��5      $
��Save��65   $
��Left=�6C   $	��140��6^   6j   m� 5      $
��Exit��6   $
��Left=�6)   $	��140��6D   5      m� 5      $��Security��6   .@�5      m� -   5        $ menuItemChangePassword��5      $��Change Password��6;   $
��Left=�6T   $	��186��6o   $��	0,0,-46,0q�6{   6�   m� 5      -   5        $ menuItemExportHash��5      $��Export Hash��67   $
��Left=�6L   $	��167��6g   $��	0,0,-27,0q�6s   6�   5      5      �� 5   
   -   5        $ btnRemoveItem��5      $��- Remove Selected��6(   $
��Left=�6D   $��
90,262,0,0q�6_   $	��Top=�6s   $	��110��6�   6�   � 5    
   -	   5        $ gridSearchArea��5       $
��Left=�6'   $��15��6B   $��205,267,0,0q�6N   $	��Top=�6c   $	��302��6{    � ColumnDefinitions 5!      �� 5"      $	��180 �6#   �� 5#      $��30 �6#   �� 5$      $��92 �6#   .+�5&      �� -
   5        $ 	btnSearch��5&      $��Search��6(   $
��Left=�69   $��3,-5,0,0q�6T   $	��Top=�6f   $��89��6~   $��2��6�   $��20��6�   �� 5'      -   5         ��Name$ tbSearch��5'      $
��Left=�6(   $��20��6C   $��0,-5,0,0q�6O    �� TextWrapping$
 Wrap=�6a   $	��Top=�6u   $	��180��6�   $��1,1,0,1q�6�   ���	#FF7070706�   �� 5(      -   5        $ btnClearSearch��5(      $��X��6-   $
��Left=�69   $��0,-5,0,0q�6T   $	��Top=�6f   $��28��6~   $��1��6�   $��0,1,1,1q�6�   6�   5)      5+      5,      