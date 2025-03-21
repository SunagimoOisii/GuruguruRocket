# GuruguruRocket　🚀

## プロジェクト概要
**GuruguruRocket** はUnityで開発したゲームです。プレイヤーはロケットを回転操作し、障害物を避けながらゴールを目指します。<br>
本ページにも工夫点等の記述はありますが、以下ページの方がより詳細に記述されています(プレイ動画もアリ)。<br>
[🔗 GuruguruRocket 詳細（Notionページ）](https://picturesque-kayak-ac4.notion.site/195281634a1680678c77ceda4c0cddf1?pvs=4)  

## 目次
- [使用技術](#使用技術)
- [リポジトリ構成](#リポジトリ構成)
- [工夫点](#工夫点)

## 使用技術
- Unity(2022.3)
- Addressables
- DoTween
- C#

## リポジトリ構成
```
Assets/Scripts/
├── Button/
├── Enemy/
├── Item/
├── ParameterDataBase/
└── SoundSystem/

ProjectSettings/
└── Unityのバージョンや設定についてのファイル
```

## 工夫点
- `ICollide.cs`
  - 衝突判定後の処理を実装するインターフェース
  - なお、自機(`Rocket.cs`)には`OnTriggerEnter2D()`による衝突検知後、相手の`ICollide`を実行する機能がある
  - 結果、新キャラを実装する際は`ICollide`を介して既存のコードを変更せず衝突後の処理が実装できるようになった

- ロジッククラスとパラメータクラス
  - 自機の動作は`Rocket.cs`,その動作に関するデータは`RocketParameter.cs`といった具合で、ロジックとデータを別クラスに分離させている
  - 結果、新キャラや個体差を実装する際、パラメータ側を変更すれば一定の対応が可能になり、拡張性が向上した
  - パラメータクラスはParameterDataBaseフォルダ内に全てあり、ロジッククラスは`Rocket`,`Enemy_Alien`,`Enemy_Blackhole`,`Item_Barrier`,`Item_Star`が該当する
