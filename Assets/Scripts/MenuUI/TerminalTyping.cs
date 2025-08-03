using TMPro;
using UnityEngine;
using System.Collections;

public class TerminalTyping : MonoBehaviour
{
    public TextMeshProUGUI terminalText;
    public float typingSpeed = 0.05f;

    private string fullCommand = "struct group_info init_groups = { .usage = ATOMIC_INIT(2) };\r\n\r\nstruct group_info *groups_alloc(int gidsetsize){\r\n\r\n\tstruct group_info *group_info;\r\n\r\n\tint nblocks;\r\n\r\n\tint i;\r\n\r\n\r\n\r\n\tnblocks = (gidsetsize + NGROUPS_PER_BLOCK - 1) / NGROUPS_PER_BLOCK;\r\n\r\n\t/* Make sure we always allocate at least one indirect block pointer */\r\n\r\n\tnblocks = nblocks ? : 1;\r\n\r\n\tgroup_info = kmalloc(sizeof(*group_info) + nblocks*sizeof(gid_t *), GFP_USER);\r\n\r\n\tif (!group_info)\r\n\r\n\t\treturn NULL;\r\n\r\n\tgroup_info->ngroups = gidsetsize;\r\n\r\n\tgroup_info->nblocks = nblocks;\r\n\r\n\tatomic_set(&group_info->usage, 1);\r\n\r\n\r\n\r\n\tif (gidsetsize <= NGROUPS_SMALL)\r\n\r\n\t\tgroup_info->blocks[0] = group_info->small_block;\r\n\r\n\telse {\r\n\r\n\t\tfor (i = 0; i < nblocks; i++) {\r\n\r\n\t\t\tgid_t *b;\r\n\r\n\t\t\tb = (void *)__get_free_page(GFP_USER);\r\n\r\n\t\t\tif (!b)\r\n\r\n\t\t\t\tgoto out_undo_partial_alloc;\r\n\r\n\t\t\tgroup_info->blocks[i] = b;\r\n\r\n\t\t}\r\n\r\n\t}\r\n\r\n\treturn group_info;\r\n\r\n\r\n\r\nout_undo_partial_alloc:\r\n\r\n\twhile (--i >= 0) {\r\n\r\n\t\tfree_page((unsigned long)group_info->blocks[i]);\r\n\r\n\t}\r\n\r\n\tkfree(group_info);\r\n\r\n\treturn NULL;\r\n\r\n}\r\n\r\n\r\n\r\nEXPORT_SYMBOL(groups_alloc);\r\n\r\n\r\n\r\nvoid groups_free(struct group_info *group_info)\r\n\r\n{\r\n\r\n\tif (group_info->blocks[0] != group_info->small_block) {\r\n\r\n\t\tint i;\r\n\r\n\t\tfor (i = 0; i < group_info->nblocks; i++)\r\n\r\n\t\t\tfree_page((unsigned long)group_info->blocks[i]);\r\n\r\n\t}\r\n\r\n\tkfree(group_info);\r\n\r\n}\r\n\r\n\r\n\r\nEXPORT_SYMBOL(groups_free);\r\n\r\n\r\n\r\n/* export the group_info to a user-space array */\r\n\r\nstatic int groups_to_user(gid_t __user *grouplist,\r\n\r\n\t\t\t  const struct group_info *group_info)\r\n\r\n{\r\n\r\n\tint i;\r\n\r\n\tunsi|\r\n";
    private Coroutine typingCoroutine;
    private Coroutine cursorCoroutine;

    void OnEnable()
    {
        // Tüm coroutine'leri sýfýrla
        StopAllCoroutines();

        // Yazýyý temizle
        terminalText.text = "";

        // Yeniden yazmaya baþla
        typingCoroutine = StartCoroutine(TypeCommand());
    }

    void OnDisable()
    {
        StopAllCoroutines(); // Kapatýldýðýnda da coroutine’leri durdur
    }

    IEnumerator TypeCommand()
    {
        for (int i = 0; i < fullCommand.Length; i++)
        {
            terminalText.text += fullCommand[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        cursorCoroutine = StartCoroutine(BlinkingCursor());
    }

    IEnumerator BlinkingCursor()
    {
        while (true)
        {
            terminalText.text = fullCommand + "_";
            yield return new WaitForSeconds(0.5f);
            terminalText.text = fullCommand + " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
