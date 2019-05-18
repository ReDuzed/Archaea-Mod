using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Entities
{
    public class ArchaeaEntity : Entity
    {
        public bool netUpdate;
        public int type;
        public int owner;
        public float rotation;
        public float[] ai = new float[2];
        public static ArchaeaEntity[] entity = new ArchaeaEntity[1001];
        public new virtual Vector2 Center
        {
            get { return new Vector2(position.X - width / 2, position.Y - height / 2); }
            set { position = new Vector2(value.X + width / 2, value.Y + height / 2); }
        }
        public new Rectangle Hitbox
        {
            get { return new Rectangle((int)position.X - width, (int)(position.Y - height / 1.5f), width, height / 2); }
        }
        public Texture2D texture;
        public static ArchaeaEntity NewEntity(Vector2 position, Vector2 velocity, int type, int owner = 255, float ai = 0f, float ai2 = 0f)
        {
            int count = 1000;
            for (int i = 0; i < entity.Length; i++)
            {
                if (entity[i] == null || !entity[i].active)
                {
                    count = i;
                    break;
                }
                if (i == count)
                {
                    return new MagnoShield();
                }
            }
            entity[count] = new MagnoShield();
            entity[count].SetDefaults();
            entity[count].whoAmI = count;
            entity[count].active = true;
            entity[count].position = position;
            entity[count].velocity = velocity;
            entity[count].type = type;
            entity[count].owner = owner;
            entity[count].ai[0] = ai;
            entity[count].ai[1] = ai2;
            return entity[count];
        }
        public virtual void SetDefaults()
        {
        }
        public virtual void Update()
        {
        }
        public virtual void Kill(bool effect)
        {
        }
    }
    public class EntityWorld : ModWorld
    {
        public override void PostUpdate()
        {
            foreach (var e in ArchaeaEntity.entity)
            {
                if (e != null)
                {
                    e.Update();
                    if (Main.netMode == 2 && e.netUpdate)
                    {
                        NetHandler.Send(Packet.SyncEntity, -1, -1, e.whoAmI, e.Center.X, e.Center.Y, 0, e.active, e.rotation);
                        if (e.type != 0)
                            e.netUpdate = false;
                    }
                }
            }
        }
        public override void PostDrawTiles()
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D shield = mod.GetTexture("Gores/MagnoShield");
            Texture2D glow = mod.GetTexture("Gores/MagnoShieldGlow");
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var e in ArchaeaEntity.entity)
            {
                if (e != null && e.active)
                {
                    sb.Draw(shield, e.Center - Main.screenPosition, null, Lighting.GetColor((int)e.Center.X / 16, (int)e.Center.Y / 16), e.rotation, new Vector2(e.width / 2, e.height / 2), 1f, SpriteEffects.None, 0f);
                    sb.Draw(glow, e.Center - Main.screenPosition, null, Color.White, e.rotation, new Vector2(e.width / 2, e.height / 2), 1f, SpriteEffects.None, 0f);
                }
            }
            sb.End();
        }
    }
    public class EntityProjectile : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            Projectile p = projectile;
            if (p.active && !p.friendly)
            {
                foreach (var e in ArchaeaEntity.entity)
                {
                    if (e != null && e.active && e.type == 0)
                    {
                        if (p.Hitbox.Intersects(e.Hitbox))
                        {
                            p.Kill();
                            e.Kill(true);
                        }
                    }
                }
            }    
        }
    }
}