require "extern"
local buff = class("buff400005");
buff.id = 0;
buff.owner = nil;
function buff:init(id,templateID,owner,caster)
	self.id = id;
	self.owner = owner;
	self.owner_id = self.owner.ID;
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
	if injured.ID == self.owner_id then
		print("half damager");
		return -value * 0.5;
	end
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