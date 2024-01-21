![License](Assets/UnityChan/License/UCL2.02/License%20Logo/Others/png/Light_Silhouette.png)

This repo is compilation of all SD model provided from https://unity-chan.com/download/

Specifically
- SD Unity-chan 3D model data
- SD Kohakuchanz
- SD Toko-chan
- SD Marie Kayano
- SD AKAZA
- SD Yuji Otori
- SD Jack

In total 15 fbx model files

All the asset was imported into project then iteratively modified into the same pattern and upgraded into newer version of various unity system

- Prefab system of unity 2022. Every prefab was now linked to the original fbx
- Convert legacy springbone of original Unitychan and Kohakuchanz into https://github.com/unity3d-jp/UnityChanSpringBone
- Rename all face mesh to the same `_face` named. And all blendshape now have `face.` prefix prepended. So every model now use the same animation/controller/mask and compatible with `OnCallChangeFace` event function
- Clean up and remove many unused files
  - Still kept some legacy script. But all script related to old springbone system was removed
 
This repo would follow and obey https://unity-chan.com/contents/guideline_en/

TODO; if possible:
- Change folder structure of package to become UPM package
- Reconfigure spring bone collider correctly (use capsule collider in arm, leg)
- Convert UnityChanSpringBone into newer system. Such as VRM or Animation.Rigging package
- Modified fbx to split apart face mesh, head/hair mesh, body mesh, accessories mesh. Because all girls body was actually the same 2 models
- Reorganize parts in texture. Implementing system to change skin color and cloth dyed
- Making all the assets become complete modular system
- Find animation for SD male character

## Disclaimer

This repo did not have official relationship with unityjp. The owner of this repo has no ownership over any asset that was released. I also do not have any right to claim nor guaranteed for anyone using these asset in their project

This project was initial in unity 2022.3 and specifically target Universal Render Pipeline. But hope to be compatible with unity HDRP if possible

Have not testing or expect to work in unity version below 2022 since the prefab connection system was introduce in this version

The sole purpose of this repo is trying to make all unity chan SD asset easier to be managed and utilized in newer version of unity, and being maintain transparently in git system

There is some additional files, mainly some messy editor script, as a tools and utility to modified asset. Those files would be temporary and might be removed if it's not necessary anymore so it would not be stick into the actual package

ps.

I am open for suggestion and contribution

Thaina Yu