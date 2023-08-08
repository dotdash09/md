import json
import sys
from ytmusicapi import YTMusic

def get_tracks(browseId, lang):
    """
    앨범 검색
    :param channelId: 앨범 아이디 (ex: MPREb_LkNaXGEfvpe)
    """    
    ytmusic = YTMusic("browser.json", language=lang)
    search_results = ytmusic.get_album(browseId)

    if not search_results:
        print("No search results found.")
    else:
        json_string = json.dumps(search_results, ensure_ascii=False, indent=2)
        print(json_string)

get_tracks(sys.argv[1], sys.argv[2])                
# get_tracks('MPREb_zY6aY0STuc5')                