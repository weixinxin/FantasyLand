require "extern"
local buff = class("buff001");
buff.id = 0;
function buff:init(id,templateID,owner,caster)
	print("init");
end

function buff:OnUnitWillDie(unit,slayer)
	print("OnUnitWillDie");
	return true;
end

function buff:OnUnitBeSlayed(unit,slayer)
	print("OnUnitBeSlayed");
end

function buff:OnUnitBeSummoned(unit,summoner)
	print("OnUnitBeSummoned");
end

function buff:OnUnitWillHurt(injured,assailant,value,dt,isAttack)
	print("OnUnitWillHurt");
	return 0;
end

function buff:OnUnitBeHurted(injured,assailant,value,dt,isAttack)
	print("OnUnitBeHurted");
end

function buff:OnUnitWillHeal(injured,healer,value)
	print("OnUnitWillHeal");
	return 0;
end

function buff:OnUnitBeHealed(injured,healer,value)
	print("OnUnitWillHeal");
end

function buff:OnUnitCastSpell(unit,skill)
	print("OnUnitCastSpell");
end

function buff:OnSpellHit(unit,caster,skill,killed)
	print("OnSpellHit");
end

return buff;