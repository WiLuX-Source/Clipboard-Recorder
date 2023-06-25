from kivy.app import App
from kivy.uix.gridlayout import GridLayout
from kivy.lang.builder import Builder
from kivy.core.clipboard import Clipboard
from kivy.config import Config
from kivy.clock import Clock
from kivy.core.image import Image as CoreImage
from PIL import ImageGrab, Image
from io import BytesIO
from datetime import datetime
import os
Builder.load_file('./App.kv')
Config.set('graphics','resizable',0)
Config.set('input','mouse','mouse,multitouch_on_demand')
Config.set('kivy','exit_on_escape',0)
Config.set('graphics','height',500)
Config.set('graphics','width',400)
Config.set('graphics','always_on_top',1)
class UpperControls(GridLayout):
    temp_clipboard = ''
    temp_img = Image.new(mode="RGB", size=(10, 10))
    def RecordClipboard(self,root):
        if ImageGrab.grabclipboard() != None and list(self.temp_img.getdata()) != list(ImageGrab.grabclipboard().getdata()):
            img = ImageGrab.grabclipboard()
            self.temp_img = img
            images = BytesIO()
            img.save(images,format='png')
            img.save(f"Images/{datetime.now().strftime('%d.%m.%Y %H-%M-%S')}.png",format='png')
            images.seek(0)
            self.ids.img.texture = CoreImage(images, ext='png').texture
            self.ids.last_item.opacity = 0
            self.ids.img.opacity = 1
        elif Clipboard.paste() != self.temp_clipboard and Clipboard.paste() != '' and Clipboard.paste().find(self.children[1].ids.keyword.text) != -1:
            self.ids.img.opacity = 0
            self.ids.last_item.opacity = 1
            self.temp_clipboard = Clipboard.paste()
            self.ids.last_item.text = Clipboard.paste()
            with open('History.txt', '+a') as output:
                try:
                    output.write("%s\n" % str(Clipboard.paste()))
                except:
                    output.write("%s\n" % str(Clipboard.paste().encode('UTF-8')))

    def ButtonToggle(self,root):
        if root.state == "normal":
            root.text = "Record OFF"
            self._Timer.cancel()
        else:
            root.text = "Record ON"
            self._Timer = Clock.schedule_interval(self.RecordClipboard,1)

class Controls(GridLayout):
    def Keyword_set(self):
        input_box = self.ids.keyword
        input_label = self.ids.display_keyword
        input_label.text = input_box.text

class ClipboardApp(App):
    def build(self):
        self.title = "WiLuX Clipboard Recorder"
        return UpperControls()

if not os.path.exists("Images"):
    os.makedirs("Images")

ClipboardApp().run()