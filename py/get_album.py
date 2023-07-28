import json
import sys
from ytmusicapi import YTMusic

def get_album(browseId):
    """
    앨범 검색
    :param channelId: 앨범 아이디 (ex: MPREb_LkNaXGEfvpe)
    """    
    # ytmusic = YTMusic("browser.json", language="en")
    ytmusic = YTMusic("browser.json", language="ko")
    search_results = ytmusic.get_album(browseId)

    if not search_results:
        print("No search results found.")
    else:
        json_string = json.dumps(search_results, ensure_ascii=False, indent=2)
        print(json_string)

get_album(sys.argv[1])                
# get_album('MPREb_G0xbuzk9a50')                