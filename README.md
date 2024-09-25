## Git Bashを使う場合の流れ
Gitのインストール
参考: https://qiita.com/T-H9703EnAc/items/4fbe6593d42f9a844b1c

Git Bashというアプリを開く  
黒い画面が出てくるので、ここにコマンドを入力する  
  
まずGitHubにSSH接続をする   
参考: https://qiita.com/shizuma/items/2b2f873a0034839e47ce

### 初回のみ

現在のディレクトリを確認する(お好みで)
```
pwd
```

Clone(GitHubからプロジェクトをコピー)する
```
git clone git@github.com:Conken-NitKit/cake-cutting.git
```

Cloneしたディレクトリに移動する  
(Windowsと異なり、ディレクトリの区切りにスラッシュ'/'を用い、ドライブレターは"/c/"のようになる  
例: `/c/Users/UserName/ProjectName`)
```
cd [ディレクトリ]
```

Unity Hubを開き、右上の"追加"を押す  
Cloneしたフォルダを選択するとUnityプロジェクトが開く  
(バージョンに注意。今回は2022.3.44f1)  

### 作業時

作業の前に今いるブランチ(履歴の流れの分岐)を確認する  
featureブランチであることを確認する  
```
git status
```

今いるブランチが間違っている場合、チェックアウト(ブランチの移動)をする  
```
git checkout [ブランチ名]
```
ブランチを新しく作る場合、-bオプションを付ける  
```
git checkout -b [ブランチ名]
```
既存のブランチ一覧を確認するにはgit branchコマンドを使う  
```
git branch
```
  
～作業する～  
  
作業内容をステージング(記録の対象を選択)する   
ここでは.を指定することで現在のフォルダ全体を選択  
```
git add .
```
  
ステージングしたファイルを確認する  
```
git status
```
   
ステージングした内容をコミット(作業内容を記録)する  
今回コミットメッセージは日本語で"{熟語}:{作業内容}"の形式に統一する  
例:  
"追加:README.mdを追加"  
"修正:コインの増殖バグを修正"  
"変更:効率化のためXXのアルゴリズムを変更しました"  
コミットメッセージの書き方参考: https://qiita.com/konatsu_p/items/dfe199ebe3a7d2010b3e  
```
git commit -m "[コミットメッセージ]"
```
  
プッシュ(リモートリポジトリに反映する)する  
```
git push -u origin [ブランチ名]
```
  
プルリクエスト(自分の変更をリモートリポジトリに反映する提案を送る)を送る  
GitHubでリポジトリを見るとPull requestsのボタンが出てくるので押す  
作業内容を書いてCreate Pull requestボタンを押す  

### リモートリポジトリとの同期

適宜プル(リモートリポジトリの変更点をダウンロード)をする  
```
git pull origin [ブランチ名]
```
