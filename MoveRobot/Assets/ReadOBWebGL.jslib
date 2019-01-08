var ReadOBPlugin = {
  ReadOBJS: function() {
    
    //var ret = 'motor(100,100);';

    var ret = document.getElementById('obstacle').value;

    var size = lengthBytesUTF8(ret) + 1; // null文字終端となるため+1
    var ptr = _malloc(size);
    // マニュアルではいまだにwriteStringToMemory()を使用したコードを記載しているが
    // 実際に実行すると代わりにstringToUTF8()を使えと怒られる
    stringToUTF8(ret, ptr, size); // この関数でHEAPxxxに書き込まれる
    return ptr;
  }
}
mergeInto(LibraryManager.library, ReadOBPlugin);