# Texture2D Array Maker (Texture2Dアレイメーカー)

## 概要

Texture2D Array Makerは、Unity用のScriptableObjectベースのユーティリティで、`Texture2DArray`アセットの作成と更新を簡素化します。`Texture2DArray`は、同じサイズとフォーマットの複数の2Dテクスチャ（スライス）を単一のテクスチャオブジェクト内に保持するGPUリソースです。これは一般的に、アニメーション用のテクスチャアトラス、地形のスプラットマップ、マテリアルのバリエーションなどのテクニックで、テクスチャの切り替えを減らしてレンダリングを最適化するために使用されます。

このツールを使用すると、入力となる`Texture2D`アセットのリストを定義し、それらから`Texture2DArray`を生成または更新することができます。

## 主な機能

-   **ScriptableObjectによる設定:**
    -   プロジェクト内に作成する各`Texture2DArrayMaker`アセットは、特定の`Texture2DArray`を生成するための設定を保持します。
    -   `Assets > Create > Texture2DArrayMaker` から新しい設定を作成します。
-   **入力テクスチャリスト:**
    -   `Texture2DArray`のスライスとなる`Texture2D`アセットのリストを指定します。
-   **出力`Texture2DArray`:**
    -   オプションで、更新する既存の`Texture2DArray`アセットを割り当てることができます。
    -   割り当てられていない場合は、新しい`Texture2DArray`アセットが作成されます。
-   **検証機能:**
    -   作成前に、ツールはすべての入力テクスチャが以下の条件を満たしているかチェックします:
        -   同じ幅と高さを持っていること。
        -   同じ`TextureFormat`であること。
    -   検証に失敗した場合、コンソールに警告が記録されます。
-   **作成と更新:**
    -   `Texture2DArrayMaker`アセットのコンテキストメニュー項目「Create or Update Texture2DArray」がプロセスをトリガーします。
    -   出力アセットが指定されていない場合、メーカーアセットと同じディレクトリに `[メーカーアセット名]_TexArray.asset` という名前で新しい`Texture2DArray`が作成されます。
    -   出力アセットが指定されている場合、その内容は新しい配列データで更新されます。
    -   各入力テクスチャのピクセルデータは、`Texture2DArray`の対応するスライスにコピーされます。

## 使い方

1.  **`Texture2DArrayMaker`アセットの作成:**
    *   Unityのプロジェクトウィンドウで右クリックします。
    *   `Create > Texture2DArrayMaker` に移動します。
    *   新しく作成されたScriptableObjectアセットに説明的な名前を付けます（例: `FlameAnimation_ArrayMaker`）。

2.  **メーカーアセットの設定:**
    *   作成した`Texture2DArrayMaker`アセットを選択します。
    *   インスペクターで:
        *   **Input Textures:** スライスとして含めたい`Texture2D`アセットをこのリストにドラッグアンドドロップします。
            *   **重要:** このリスト内のすべてのテクスチャは、**必ず**同じ寸法（幅と高さ）と同じ`TextureFormat`を持っている必要があります。ツールはリストの最初のテクスチャをこれらのプロパティの参照として使用します。
        *   **Output Texture (任意):**
            *   既存の`Texture2DArray`アセットを更新したい場合は、それをこのフィールドにドラッグします。
            *   新しい`Texture2DArray`アセットを作成したい場合は、このフィールドを空（None）のままにします。

3.  **`Texture2DArray`の生成または更新:**
    *   プロジェクトウィンドウで`Texture2DArrayMaker`アセットが選択されていることを確認します。
    *   選択した`Texture2DArrayMaker`アセットを右クリックします。
    *   表示されるコンテキストメニューから **「Create or Update Texture2DArray」** を選択します。

4.  **結果の確認:**
    *   「Output Texture」フィールドが空だった場合:
        *   `Texture2DArrayMaker`アセットと同じフォルダに新しい`Texture2DArray`アセットが作成されます。その名前はメーカーの名前に基づいて付けられます（例: `FlameAnimation_ArrayMaker_TexArray.asset`）。
        *   `Texture2DArrayMaker`アセットの「Output Texture」フィールドには、この新しく作成されたアレイへの参照が自動的に入力されます。
    *   「Output Texture」フィールドが既存の`Texture2DArray`を指していた場合:
        *   そのアセットの内容が、入力テクスチャからの新しいスライスで更新されます。
    *   入力テクスチャが検証基準（同じサイズとフォーマット）を満たしていなかった場合は特に、Unityコンソールで警告メッセージを確認してください。

このツールを使用することで、ソーステクスチャが変更されたときに`Texture2DArray`アセットを簡単に管理および再生成でき、さまざまな高度なレンダリングテクニックや最適化に役立ちます。
