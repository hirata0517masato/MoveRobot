開発環境
OS: Windows10
Unity: 5.5.2f1 (64-bit)

使い方
MoveRobot/Assets/ReadPath.txt
に記述されたファイルを読み込み、
そこに記述されたソースコードをUnityから実行

実行形式の場合
MoveRobot.exeと同じパスのReadPath.txt
に記述されたファイルを読み込み、
そこに記述されたソースコードを実行


主な対応機能
変数：int,double
条件分岐:if
ループ：for,while
コメント：/* */
ロボット操作：motor関数に左右の出力を与える（-100から100）

四則演算や条件式に対応
+=や++に対応（ただし、後置、前置に意味はない）