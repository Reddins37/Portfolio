# -*- mode: python -*-

block_cipher = None


a = Analysis(['TrafficTool.py'],
             pathex=['C:\\Users\\reddi\\Documents\\School\\Summer 2018\\IoTTrafficTool'],
             binaries=[],
             datas=[
			 ('lib/site-packages/netdisco/discoverables/*', 'netdisco/discoverables'),
			 ('lib/site-packages/pyshark/config.ini', 'pyshark')
			 ],
             hiddenimports=['py._path.local', 'py._vendored_packages.iniconfig', 'pyshark.config'],
             hookspath=[],
             runtime_hooks=[],
             excludes=[],
             win_no_prefer_redirects=False,
             win_private_assemblies=False,
             cipher=block_cipher)
pyz = PYZ(a.pure, a.zipped_data,
             cipher=block_cipher)
exe = EXE(pyz,
          a.scripts,
          a.binaries,
          a.zipfiles,
          a.datas,
          name='TrafficTool',
          debug=False,
          strip=False,
          upx=True,
          runtime_tmpdir=None,
          console=False )
