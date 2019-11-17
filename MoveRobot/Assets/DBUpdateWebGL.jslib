var DBUpdatePlugin = {
  DbUpdateJS: function() {

    /*if(typeof MissionCompleat == 'function') {
      MissionCompleat();
      return "0";
    }*/
    MissionCompleat();
    return "1";
  }
}
mergeInto(LibraryManager.library, DBUpdatePlugin);