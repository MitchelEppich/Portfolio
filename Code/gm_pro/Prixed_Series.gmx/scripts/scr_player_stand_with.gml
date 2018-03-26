/// scr_player_stand()

var sprite = spr_human_stand_with;
var weapon_sprite = spr_knife_stand_with;

if (sprite_index != sprite) sprite_index = sprite;
if (weapon_sprite_index != weapon_sprite) weapon_sprite_index = weapon_sprite;

image_speed = 0.1;

if (abs(hsp) > 0) x_scale = sign(hsp);
